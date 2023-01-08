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
                                   bool watchInserts = false,
                                   bool watchUpdates = false,
                                   bool watchDeletes = false)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.WatchInserts = watchInserts;
      this.WatchUpdates = watchUpdates;
      this.WatchDeletes = watchDeletes;
    }

    public string ConnectionString { get; private set; }

    public string Database { get; private set; }

    public string Collection { get; private set; }

    public bool WatchInserts { get; private set; }

    public bool WatchUpdates { get; private set; }

    public bool WatchDeletes { get; private set; }

    // Todo:
    public string Filter { get; set; }
  }
}

