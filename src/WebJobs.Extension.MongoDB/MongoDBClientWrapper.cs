using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBClientWrapper
  {
    private readonly IMongoClient mongoClient;
    private readonly ILogger logger;

    public MongoDBClientWrapper(string connectionString, ILogger logger)
    {
      this.logger = logger;
      this.mongoClient = new MongoClient(connectionString);
      try
      {
        var databaseName = this.mongoClient.ListDatabaseNames(); // Connectivity check
      }
      catch (MongoException)
      {
        throw new ArgumentException("Failed to connect to MongoDB. Please check if the connection string is valid or accessible from the azure frunction.");
      }
    }

    public void Watch(MongoDBTriggerAttribute attribute,
                      Action<MongoDBTriggerResponseData> callback,
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
                                          Action<MongoDBTriggerResponseData> callback,
                                          ChangeStreamOptions options,
                                          BsonDocument changeStreamPipelineMatchStage,
                                          CancellationToken cancellationToken)
    {
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>().Match(changeStreamPipelineMatchStage);
      try
      {
        this.StartChangeStream(attribute, callback, pipeline, options, cancellationToken);
        this.logger.LogInformation($"Started the change stream with pipeline : {pipeline.ToJson()}");
      }
      catch (MongoException ex)
      {
        this.logger.LogError(ex, $"Exception while starting the change stream with pipeline : {pipeline.ToJson()}");
        throw new ArgumentException("Passed invalid pipeline match stage. Please refer the documentation to see the expected format.", ex);
      }
    }

    private void WatchWithBasicMatchStage(MongoDBTriggerAttribute attribute,
                                          Action<MongoDBTriggerResponseData> callback,
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
          array.Add(new BsonDocument($"updateDescription.updatedFields.{field}", new BsonDocument("$exists", true)));
        }
      }
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>().Match(matchStage);
      this.logger.LogInformation($"Started the change stream with pipeline : {pipeline.ToJson()}");

      try
      {
        this.StartChangeStream(attribute, callback, pipeline, options, cancellationToken);
      }
      catch (MongoException ex)
      {
        this.logger.LogError(ex, $"Exception while starting the change stream with pipeline : {pipeline.ToJson()}");
        throw ex;
      }
    }

    private void StartChangeStream(MongoDBTriggerAttribute attribute,
                       Action<MongoDBTriggerResponseData> callback,
                       PipelineDefinition<ChangeStreamDocument<BsonDocument>, ChangeStreamDocument<BsonDocument>> pipeline,
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
          this.IterateCursor(callback, (IChangeStreamCursor<ChangeStreamDocument<BsonDocument>>)cursor);
        }
      }
    }

    private void IterateCursor(Action<MongoDBTriggerResponseData> callback,
                               IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> cursor)
    {
      var enumerator = cursor.ToEnumerable().GetEnumerator();
      while (enumerator.MoveNext())
      {
        ChangeStreamDocument<BsonDocument> change = enumerator.Current;
        var responseData = new MongoDBTriggerResponseData()
        {
          CollectionNamespace = change.CollectionNamespace,
          DatabaseNamespace = change.DatabaseNamespace,
          CollectionUuid = change.CollectionUuid,
          DocumentKey = change.DocumentKey,
          FullDocument = change.FullDocument,
          FullDocumentBeforeChange = change.FullDocumentBeforeChange,
          OperationType = change.OperationType,
          ResumeToken = change.ResumeToken,
          UpdateDescription = change.UpdateDescription,
          WallTime = change.WallTime
        };
        callback(responseData);
      }
    }

    private BsonArray FetchOperations(MongoDBTriggerAttribute attribute)
    {
      var operations = new BsonArray();
      if (attribute.WatchInserts)
      {
        operations.Add("insert");
      }

      if (attribute.WatchUpdates)
      {
        operations.Add("update");
      }

      if (attribute.WatchDeletes)
      {
        operations.Add("delete");
      }

      if (attribute.WatchReplaces)
      {
        operations.Add("replace");
      }

      return operations;
    }

    private List<string> ParseWatchFields(string watchFields)
    {
      if (string.IsNullOrEmpty(watchFields))
      {
        return new List<string>();
      }

      string[] values = watchFields.Split(',');
      for (int i = 0; i < values.Length; i++)
      {
        values[i] = values[i].Trim();
      }

      return values.ToList();
    }

    private BsonDocument ParseChangeStreamPipelineMatchStage(string pipeline)
    {
      if (string.IsNullOrEmpty(pipeline))
      {
        return null;
      }

      try
      {
        return BsonSerializer.Deserialize<BsonDocument>(pipeline);
      }
      catch (Exception ex)
      {
        throw new ArgumentException($"Passed invalid pipeline match stage. Please refer the documentation to see the expected format. Pipeline passed : {pipeline}", ex);
      }
    }
  }
}

