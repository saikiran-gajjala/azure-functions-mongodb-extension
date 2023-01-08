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
        throw new ArgumentException("Failed to connect to MongoDB. Please check if the connection string is valid or accessible from azure frunction.");
      }
    }

    public void Watch(MongoDBTriggerAttribute attribute,
                                 Action<MongoDBTriggerResponseData> callback,
                                 CancellationToken cancellationToken)
    {


      var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
      var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                    .Match(x => x.OperationType == ChangeStreamOperationType.Insert);
      if (!string.IsNullOrEmpty(attribute.Database) && !string.IsNullOrEmpty(attribute.Collection))
      {
        var db = this.mongoClient.GetDatabase(attribute.Database);
        var mongoCollection = db.GetCollection<BsonDocument>(attribute.Collection);
        this.WatchSingleCollectionInSingleDatabase(mongoCollection, callback, cancellationToken);
      }
      else if (!string.IsNullOrEmpty(attribute.Database) && string.IsNullOrEmpty(attribute.Collection))
      {
        var db = this.mongoClient.GetDatabase(attribute.Database);
        this.WatchAllCollectionInSingleDatabase(db, callback, cancellationToken);
      }
      else
      {
        this.WatchAllCollectionInAllDatabases(this.mongoClient, callback, cancellationToken);
      }
    }

    private void WatchSingleCollectionInSingleDatabase(IMongoCollection<BsonDocument> collection, Action<MongoDBTriggerResponseData> callback, CancellationToken cancellationToken)
    {
      using (var cursor = collection.Watch(null, cancellationToken))
      {
        this.IterateCursor(callback, cursor);
      }
    }

    private void WatchAllCollectionInSingleDatabase(IMongoDatabase database, Action<MongoDBTriggerResponseData> callback, CancellationToken cancellationToken)
    {
      using (var cursor = database.Watch(null, cancellationToken))
      {
        this.IterateCursor(callback, cursor);
      }
    }

    private void WatchAllCollectionInAllDatabases(IMongoClient client, Action<MongoDBTriggerResponseData> callback, CancellationToken cancellationToken)
    {
      using (var cursor = client.Watch(null, cancellationToken))
      {
        this.IterateCursor(callback, (IChangeStreamCursor<ChangeStreamDocument<BsonDocument>>)cursor);
      }
    }

    private void IterateCursor(Action<MongoDBTriggerResponseData> callback, IChangeStreamCursor<ChangeStreamDocument<BsonDocument>> cursor)
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
  }
}

