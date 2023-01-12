using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using Newtonsoft.Json;

namespace Hackathon.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// CosmosDB's MongoDB API client wrapper class that interacts with MongoDB using MongoDB driver.
  /// </summary>
  public class CosmosDBClientWrapper : BaseClientWrapper
  {
    public CosmosDBClientWrapper(string connectionString, ILogger logger) : base(connectionString, logger)
    {
    }

    /// <summary>
    /// Initiates the MongoDB Change Stream on the target collection(s)
    /// Note: CosmosDB has limitation with change stream with operationType & updateDescription
    /// Please refer the https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/change-streams?tabs=csharp#current-limitations for more information
    /// </summary>
    public override void Watch(MongoDBTriggerAttribute attribute,
                      Action<string> callback,
                      CancellationToken cancellationToken)
    {

      var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
      var changeStreamPipelineMatchStage = this.ParseChangeStreamPipelineMatchStage(attribute.PipelineMatchStage);
      if (changeStreamPipelineMatchStage != null)
      {
        this.WatchWithInputMatchStage(attribute, callback, options, changeStreamPipelineMatchStage, cancellationToken);
      }
      else
      {
        this.WatchWithBasicMatchStage(attribute, callback, options, cancellationToken);
      }
    }

    private void WatchWithInputMatchStage(MongoDBTriggerAttribute attribute,
                                          Action<string> callback,
                                          ChangeStreamOptions options,
                                          BsonDocument changeStreamPipelineMatchStage,
                                          CancellationToken cancellationToken)
    {
      /// CosmosDB has limitation with change stream with operationType & updateDescription. So excluding them in $project stage
      /// Please refer the https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/change-streams?tabs=csharp#current-limitations for more information
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                    .Match(changeStreamPipelineMatchStage)
                    .AppendStage<ChangeStreamDocument<BsonDocument>, ChangeStreamDocument<BsonDocument>, BsonDocument>(
                    "{ $project: { '_id': 1, 'fullDocument': 1, 'ns': 1, 'documentKey': 1 }}");
      try
      {
        this.StartChangeStream(attribute, callback, pipeline, options, cancellationToken);
        this.logger.LogInformation($"Started the CosmosDB change stream.");
      }
      catch (MongoException ex)
      {
        this.logger.LogError(ex, $"Exception while starting the CosmosDB change stream.");
        throw new ArgumentException("Passed invalid pipeline match stage. Please refer the documentation to see the expected format.", ex);
      }
    }

    private void WatchWithBasicMatchStage(MongoDBTriggerAttribute attribute,
                                          Action<string> callback,
                                          ChangeStreamOptions options,
                                          CancellationToken cancellationToken)
    {
      var operations = this.FetchOperations(attribute);
      var matchStage = new BsonDocument("operationType", new BsonDocument("$in", operations));
      var watchFields = this.ParseWatchFields(attribute.WatchFields);
      if (watchFields.Count > 0)
      {
        var array = new BsonArray();
        matchStage.AddRange(new BsonDocument("$or", array));
        foreach (var field in watchFields)
        {
          array.Add(new BsonDocument($"fullDocument.{field}", new BsonDocument("$exists", true)));
        }
      }


      /// CosmosDB has limitation with change stream with operationType & updateDescription. So excluding them in $project stage
      /// Please refer the https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/change-streams?tabs=csharp#current-limitations for more information
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                    .Match(matchStage)
                    .AppendStage<ChangeStreamDocument<BsonDocument>, ChangeStreamDocument<BsonDocument>, BsonDocument>(
                    "{ $project: { '_id': 1, 'fullDocument': 1, 'ns': 1, 'documentKey': 1 }}");

      this.logger.LogInformation($"Started the CosmosDB change stream");

      try
      {
        this.StartChangeStream(attribute, callback, pipeline, options, cancellationToken);
      }
      catch (MongoException ex)
      {
        this.logger.LogError(ex, $"Exception while starting the CosmosDB change stream");
        throw ex;
      }
    }

    private void StartChangeStream(MongoDBTriggerAttribute attribute,
                       Action<string> callback,
                       PipelineDefinition<ChangeStreamDocument<BsonDocument>, BsonDocument> pipeline,
                       ChangeStreamOptions options,
                       CancellationToken cancellationToken)
    {
      if (!string.IsNullOrEmpty(attribute.Database) && !string.IsNullOrEmpty(attribute.Collection))
      {
        // Watches a single collection in a database
        var db = this.mongoClient.GetDatabase(attribute.Database);
        var mongoCollection = db.GetCollection<BsonDocument>(attribute.Collection);
        using (var cursor = mongoCollection.Watch(pipeline, options, cancellationToken))
        {
          this.IterateCursor(callback, cursor);
        }
      }
      else if (!string.IsNullOrEmpty(attribute.Database) && string.IsNullOrEmpty(attribute.Collection))
      {
        // Watches all collections in a database
        var db = this.mongoClient.GetDatabase(attribute.Database);
        using (var cursor = db.Watch(pipeline, options, cancellationToken))
        {
          this.IterateCursor(callback, cursor);
        }
      }
      else
      {
        // Watches all collections in all databases
        using (var cursor = this.mongoClient.Watch(pipeline, options, cancellationToken))
        {
          this.IterateCursor(callback, cursor);
        }
      }
    }

    private void IterateCursor(Action<string> callback,
                               IChangeStreamCursor<BsonDocument> cursor)
    {
      var enumerator = cursor.ToEnumerable().GetEnumerator();
      while (enumerator.MoveNext())
      {
        var ns = enumerator.Current.GetValue("ns").ToBsonDocument();
        var databaseNameSpace = new DatabaseNamespace(ns.GetValue("db").ToString());
        var collectionNamespace = new CollectionNamespace(databaseNameSpace, ns.GetValue("coll").ToString());
        var responseData = new MongoDBTriggerEventData()
        {
          CollectionNamespace = collectionNamespace,
          DatabaseNamespace = databaseNameSpace,
          DocumentKey = enumerator.Current.GetValue("documentKey").ToBsonDocument(),
          FullDocument = enumerator.Current.GetValue("fullDocument").ToBsonDocument(),
        };
        var responseDataJson = JsonConvert.SerializeObject(responseData);
        callback(responseDataJson);
      }
    }

    protected override BsonArray FetchOperations(MongoDBTriggerAttribute attribute)
    {
      var operations = new BsonArray();
      /// cannot support delete operation because of the follwing limitation
      /// https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/change-streams?tabs=csharp#current-limitations
      operations.Add("insert");
      operations.Add("update");
      operations.Add("replace");
      return operations;
    }
  }
}

