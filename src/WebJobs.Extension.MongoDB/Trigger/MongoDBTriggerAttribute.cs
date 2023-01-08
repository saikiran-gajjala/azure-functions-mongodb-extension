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
                                   bool watchInserts = true,
                                   bool watchUpdates = true,
                                   bool watchDeletes = true)
    {
      this.ConnectionString = connectionString;
      this.Database = database;
      this.Collection = collection;
      this.WatchInserts = watchInserts;
      this.WatchUpdates = watchUpdates;
      this.WatchDeletes = watchDeletes;
    }

    public string ConnectionString { get; set; }

    public string Database { get; set; }

    public string Collection { get; set; }

    public bool WatchInserts { get; set; }

    public bool WatchUpdates { get; set; }

    public bool WatchDeletes { get; set; }

    // Todo:
    public string Filter { get; set; }
  }
}

