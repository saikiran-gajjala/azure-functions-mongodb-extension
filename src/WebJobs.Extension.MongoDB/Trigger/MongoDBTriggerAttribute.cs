using System;
using Microsoft.Azure.WebJobs.Description;

namespace Azure.Functions.Extension.MongoDB
{
  [AttributeUsage(AttributeTargets.Parameter)]
  [Binding]
  public class MongoDBTriggerAttribute : Attribute
  {
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

