using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;

namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBServiceFactory : IMongoDBServiceFactory
  {
    private readonly ILogger logger;
    public MongoDBServiceFactory(ILoggerFactory loggerFactory)
    {
      this.logger = loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("MongoDB"));
    }

    public MongoDBClientWrapper CreateMongoDBClient(string connectionString)
    {
      return new MongoDBClientWrapper(connectionString, this.logger);
    }
  }
}

