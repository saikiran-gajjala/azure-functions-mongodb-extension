using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;

namespace Hackathon.Azure.Functions.Extension.MongoDB
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
    public MongoDBClientWrapper CreateMongoDBClient(string connectionString)
    {
      return new MongoDBClientWrapper(connectionString, this.logger);
    }
  }
}