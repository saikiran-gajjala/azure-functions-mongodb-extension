using System;
using Microsoft.Azure.WebJobs.Description;
using MongoDB.Driver;

namespace Hackathon.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// MongoDB Trigger Attribute Class to decorate paramaters for the function run method.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  [Binding]
  public class MongoDBTriggerAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBTriggerAttribute"/> attribute class.
    /// </summary>
    /// <param name="connectionString">Connection String of the MongoDB replica-set</param>
    /// <param name="database">Name of the database to watch</param>
    /// <param name="collection">Name of the collection to watch</param>
    /// <param name="pipelineMatchStage">
    /// MongoDB's aggregation match stage in json format. The match stage can be applied on <see cref="ChangeStreamOptions"/>
    /// For eg: {"operationType": {"$in": ["update"]},"$or": [{"updateDescription.updatedFields.total": {"$exists": true}}]}
    /// </param>
    public MongoDBTriggerAttribute(string connectionString,
                                   string database,
                                   string collection,
                                   string pipelineMatchStage)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.IsCosmosDB = false;
      this.WatchInserts = false;
      this.WatchUpdates = false;
      this.WatchDeletes = false;
      this.WatchReplaces = false;
      this.PipelineMatchStage = pipelineMatchStage;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBTriggerAttribute"/> attribute class.
    /// </summary>
    /// <param name="connectionString">Connection String of the MongoDB replica-set</param>
    /// <param name="database">Name of the database to watch</param>
    /// <param name="collection">Name of the collection to watch</param>
    /// <param name="isCosmosDB">
    /// Flag if CosmosDB's MongoDB API is used.
    /// Note: CosmosDB has limitation with change stream with operationType & updateDescription so the match stage should be defined accordingly.
    /// Please refer the https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/change-streams?tabs=csharp#current-limitations for more information.
    /// </param>
    /// <param name="pipelineMatchStage">
    /// MongoDB's aggregation match stage in json format. If isCosmosDB is set to true then check the limitations.
    /// </param>
    public MongoDBTriggerAttribute(string connectionString,
                                   string database,
                                   string collection,
                                   bool isCosmosDB,
                                   string pipelineMatchStage)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.IsCosmosDB = isCosmosDB;
      this.WatchInserts = false;
      this.WatchUpdates = false;
      this.WatchDeletes = false;
      this.WatchReplaces = false;
      this.PipelineMatchStage = pipelineMatchStage;

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBTriggerAttribute"/> attribute class.
    /// </summary>
    /// <param name="connectionString">Connection String of the MongoDB replica-set</param>
    /// <param name="database">Name of the database to watch</param>
    /// <param name="collection">Name of the collection to watch</param>
    /// <param name="watchInserts">Flag to watch the insert operations</param>
    /// <param name="watchUpdates">Flag to watch the update operations</param>
    /// <param name="watchDeletes">Flag to watch the delete operations</param>
    /// <param name="watchReplaces">Flag to watch the replace operations</param>
    public MongoDBTriggerAttribute(string connectionString,
                                   string database,
                                   string collection,
                                   bool watchInserts = true,
                                   bool watchUpdates = true,
                                   bool watchDeletes = true,
                                   bool watchReplaces = true)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.IsCosmosDB = false;
      this.WatchInserts = watchInserts;
      this.WatchUpdates = watchUpdates;
      this.WatchDeletes = watchDeletes;
      this.WatchReplaces = watchReplaces;
      this.PipelineMatchStage = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBTriggerAttribute"/> attribute class.
    /// </summary>
    /// <param name="connectionString">Connection String of the MongoDB replica-set</param>
    /// <param name="database">Name of the database to watch</param>
    /// <param name="collection">Name of the collection to watch</param>
    /// <param name="isCosmosDB">
    /// Flag if CosmosDB's MongoDB API is used.
    /// Note: CosmosDB has limitation with change stream with operationType & updateDescription so the match stage should be defined accordingly.
    /// Please refer the https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/change-streams?tabs=csharp#current-limitations for more information.
    /// </param>
    public MongoDBTriggerAttribute(string connectionString,
                                   string database,
                                   string collection,
                                   bool isCosmosDB)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.IsCosmosDB = isCosmosDB;
      // Note: Set the following boolean flags based on the limitation mentioned above.
      this.WatchInserts = true;
      this.WatchUpdates = true;
      this.WatchDeletes = false;
      this.WatchReplaces = true;
      this.PipelineMatchStage = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBTriggerAttribute"/> attribute class.
    /// </summary>
    /// <param name="connectionString">Connection String of the MongoDB replica-set</param>
    /// <param name="database">Name of the database to watch</param>
    /// <param name="collection">Name of the collection to watch</param>
    /// <param name="isCosmosDB">
    /// Flag if CosmosDB's MongoDB API is used.
    /// Note: CosmosDB has limitation with change stream with operationType & updateDescription so the match stage should be defined accordingly.
    /// Please refer the https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/change-streams?tabs=csharp#current-limitations for more information.
    /// </param>
    /// <param name="watchInserts">Flag to watch the insert operations</param>
    /// <param name="watchUpdates">Flag to watch the update operations</param>
    /// <param name="watchDeletes">Flag to watch the delete operations</param>
    /// <param name="watchReplaces">Flag to watch the replace operations</param>
    /// <param name="pipelineMatchStage">
    /// MongoDB's aggregation match stage in json format. The match stage can be applied on <see cref="ChangeStreamOptions"/>
    /// For eg: {"operationType": {"$in": ["update"]},"$or": [{"updateDescription.updatedFields.total": {"$exists": true}}]}
    /// Is isCosmosDB is set to true then check the limitation of CosmosDB
    /// </param>
    public MongoDBTriggerAttribute(string connectionString,
                                   string database,
                                   string collection,
                                   bool isCosmosDB,
                                   bool watchInserts = true,
                                   bool watchUpdates = true,
                                   bool watchDeletes = true,
                                   bool watchReplaces = true,
                                   string pipelineMatchStage = null)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.IsCosmosDB = isCosmosDB;
      this.WatchInserts = watchInserts;
      this.WatchUpdates = watchUpdates;
      this.WatchDeletes = watchDeletes;
      this.WatchReplaces = watchReplaces;
      this.PipelineMatchStage = pipelineMatchStage;
    }

    public string ConnectionString { get; set; }

    public string Database { get; set; }

    public string Collection { get; set; }

    public bool WatchInserts { get; private set; }

    public bool WatchUpdates { get; private set; }

    public bool WatchDeletes { get; private set; }

    public bool WatchReplaces { get; private set; }

    public string PipelineMatchStage { get; set; }

    public bool IsCosmosDB { get; set; }
  }
}