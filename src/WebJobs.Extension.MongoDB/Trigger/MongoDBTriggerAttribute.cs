using System;
using Microsoft.Azure.WebJobs.Description;
using MongoDB.Driver;

namespace Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// MongoDB Trigger Attribute Class to decorate paramaters for the function run method.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  [Binding]
  public class MongoDBTriggerAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBTriggerAttribute"/> attribue class.
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
      this.WatchInserts = false;
      this.WatchUpdates = false;
      this.WatchDeletes = false;
      this.WatchReplaces = false;
      this.WatchFields = null;
      this.PipelineMatchStage = pipelineMatchStage;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBTriggerAttribute"/> attribue class.
    /// </summary>
    /// <param name="connectionString">Connection String of the MongoDB replica-set</param>
    /// <param name="database">Name of the database to watch</param>
    /// <param name="collection">Name of the collection to watch</param>
    /// <param name="watchInserts">Flag to watch the insert operations</param>
    /// <param name="watchUpdates">Flag to watch the update operations</param>
    /// <param name="watchDeletes">Flag to watch the delete operations</param>
    /// <param name="watchReplaces">Flag to watch the replace operations</param>
    /// <param name="watchFields">
    /// List of fields in the target MongoDB document to watch.
    /// Specify the multiple fields by seperating with comma. For e.g "field1,field2,field3".
    /// </param>
    public MongoDBTriggerAttribute(string connectionString,
                                   string database,
                                   string collection,
                                   bool watchInserts = true,
                                   bool watchUpdates = true,
                                   bool watchDeletes = true,
                                   bool watchReplaces = true,
                                   string watchFields = null)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.WatchInserts = watchInserts;
      this.WatchUpdates = watchUpdates;
      this.WatchDeletes = watchDeletes;
      this.WatchReplaces = watchReplaces;
      this.WatchFields = watchFields;
      this.PipelineMatchStage = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBTriggerAttribute"/> attribue class.
    /// </summary>
    /// <param name="connectionString">Connection String of the MongoDB replica-set</param>
    /// <param name="database">Name of the database to watch</param>
    /// <param name="collection">Name of the collection to watch</param>
    /// <param name="watchInserts">Flag to watch the insert operations</param>
    /// <param name="watchUpdates">Flag to watch the update operations</param>
    /// <param name="watchDeletes">Flag to watch the delete operations</param>
    /// <param name="watchReplaces">Flag to watch the replace operations</param>
    /// <param name="watchFields">
    /// List of fields in the target MongoDB document to watch.
    /// Specify the multiple fields by seperating with comma. For e.g "field1,field2,field3"
    /// </param>
    /// <param name="pipelineMatchStage">
    /// MongoDB's aggregation match stage in json format. The match stage can be applied on <see cref="ChangeStreamOptions"/>
    /// For eg: {"operationType": {"$in": ["update"]},"$or": [{"updateDescription.updatedFields.total": {"$exists": true}}]}
    /// </param>
    public MongoDBTriggerAttribute(string connectionString,
                                   string database,
                                   string collection,
                                   bool watchInserts = true,
                                   bool watchUpdates = true,
                                   bool watchDeletes = true,
                                   bool watchReplaces = true,
                                   string watchFields = null,
                                   string pipelineMatchStage = null)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.WatchInserts = watchInserts;
      this.WatchUpdates = watchUpdates;
      this.WatchDeletes = watchDeletes;
      this.WatchReplaces = watchReplaces;
      this.WatchFields = watchFields;
      this.PipelineMatchStage = pipelineMatchStage;
    }

    public string ConnectionString { get; set; }

    public string Database { get; set; }

    public string Collection { get; set; }

    public bool WatchInserts { get; private set; }

    public bool WatchUpdates { get; private set; }

    public bool WatchDeletes { get; private set; }

    public bool WatchReplaces { get; private set; }

    public string WatchFields { get; set; }

    public string PipelineMatchStage { get; set; }
  }
}

