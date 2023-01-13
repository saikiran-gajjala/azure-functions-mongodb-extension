using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Hackathon.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// MongoDB client wrapper class that interacts with MongoDB using MongoDB driver.
  /// </summary>
  public class MongoDBClientWrapper : BaseClientWrapper
  {
    public MongoDBClientWrapper(string connectionString, ILogger logger) : base(connectionString, logger) { }

    public MongoDBClientWrapper(IMongoClient mongoClient, ILogger logger) : base(mongoClient, logger) { }

    /// <summary>
    /// Initiates the MongoDB Change Stream on the target collection(s)
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
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>().Match(changeStreamPipelineMatchStage);
      try
      {
        this.StartChangeStream(attribute, callback, pipeline, options, cancellationToken);
        this.logger.LogInformation($"Started the MongoDB change stream");
      }
      catch (MongoException ex)
      {
        this.logger.LogError(ex, $"Exception while starting the MongoDB change stream");
        throw new ArgumentException("Passed invalid pipeline match stage. Please refer the documentation to see the expected format", ex);
      }
    }

    private void WatchWithBasicMatchStage(MongoDBTriggerAttribute attribute,
                                          Action<string> callback,
                                          ChangeStreamOptions options,
                                          CancellationToken cancellationToken)
    {
      var operations = this.FetchOperations(attribute);
      var operationDoc = new BsonDocument("operationType", new BsonDocument("$in", operations));
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>().Match(operationDoc);
      this.logger.LogInformation($"Started the MongoDB change stream.Mongo");

      try
      {
        this.StartChangeStream(attribute, callback, pipeline, options, cancellationToken);
      }
      catch (MongoException ex)
      {
        this.logger.LogError(ex, $"Exception while starting the MongoDB change stream");
        throw ex;
      }
    }

    private void StartChangeStream(MongoDBTriggerAttribute attribute,
                       Action<string> callback,
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
          this.IterateCursor(callback, cursor);
        }
      }
    }

    private void IterateCursor(Action<string> callback,
                               IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> cursor)
    {
      var enumerator = cursor.ToEnumerable().GetEnumerator();
      while (enumerator.MoveNext())
      {
        ChangeStreamDocument<BsonDocument> change = enumerator.Current;
        var responseData = new MongoDBTriggerEventData()
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
        var responseDataJson = JsonConvert.SerializeObject(responseData);
        callback(responseDataJson);
      }
    }
  }
}

