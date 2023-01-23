using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;

namespace Custom.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Implementation class for <see cref="IMongoDBServiceFactory"/> interface.
  /// </summary>
  public class MongoDBServiceFactory : IMongoDBServiceFactory
  {
    private readonly ILogger logger;
    public MongoDBServiceFactory(ILoggerFactory loggerFactory)
    {
      this.logger = loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("MongoDB"));
    }

    /// <summary>
    /// Create MongoDB Client Wrapper instance from connection string
    /// </summary>
    public BaseClientWrapper CreateMongoDBClient(string connectionString, bool isCosmosDB)
    {
      if (isCosmosDB)
      {
        return new CosmosDBClientWrapper(connectionString, this.logger);
      }
      else
      {
        return new MongoDBClientWrapper(connectionString, this.logger);
      }
    }
  }
}