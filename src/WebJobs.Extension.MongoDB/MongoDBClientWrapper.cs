using System;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBClientWrapper
  {
    private IMongoClient mongoClient;

    public MongoDBClientWrapper(string connectionString)
    {
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
      BsonArray operations = this.FetchOperations(attribute);
      var matchStage = new BsonDocument("operationType",
                  new BsonDocument("$in", operations));
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
               .Match(matchStage);
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
  }
}

