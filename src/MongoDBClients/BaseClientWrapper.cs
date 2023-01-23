using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Custom.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Base class for client wrappers that interacts with MongoDB using MongoDB driver.
  /// </summary>
  public abstract class BaseClientWrapper
  {
    protected readonly IMongoClient mongoClient;
    protected readonly ILogger logger;

    public BaseClientWrapper(string connectionString, ILogger logger)
    {
      this.logger = logger;
      this.mongoClient = new MongoClient(connectionString);
      try
      {
        var databaseName = this.mongoClient.ListDatabaseNames(); // Connectivity check
      }
      catch (MongoException)
      {
        throw new ArgumentException("Failed to connect to MongoDB. Please check if the connection string is valid or accessible from the azure function.");
      }
    }

    public BaseClientWrapper(IMongoClient mongoClient, ILogger logger)
    {
      this.logger = logger;
      this.mongoClient = mongoClient;
      try
      {
        var databaseName = this.mongoClient.ListDatabaseNames(); // Connectivity check
      }
      catch (MongoException)
      {
        throw new ArgumentException("Failed to connect to MongoDB. Please check if the connection string is valid or accessible from the azure function.");
      }
    }

    /// <summary>
    /// Initiates the MongoDB Change Stream on the target collection(s)
    /// </summary>
    public abstract void Watch(MongoDBTriggerAttribute attribute,
                      Action<string> callback,
                      CancellationToken cancellationToken);

    protected virtual BsonArray FetchOperations(MongoDBTriggerAttribute attribute)
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

    protected BsonDocument ParseChangeStreamPipelineMatchStage(string pipeline)
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

