using Microsoft.Azure.WebJobs.Host.Config;

namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBExtensionConfigProvider : IExtensionConfigProvider
  {
    private readonly IMongoDBServiceFactory mongoDBServiceFactory;

    public MongoDBExtensionConfigProvider(IMongoDBServiceFactory mongoDBServiceFactory)
    {
      this.mongoDBServiceFactory = mongoDBServiceFactory;
    }

    public void Initialize(ExtensionConfigContext context)
    {
      var triggerRule = context.AddBindingRule<MongoDBTriggerAttribute>();
      triggerRule.BindToTrigger(new MongoDBTriggerBindingProvider(this));
    }

    public MongoDBTriggerContext CreateContext(MongoDBTriggerAttribute attribute)
    {
      return new MongoDBTriggerContext(attribute, mongoDBServiceFactory.CreateMongoDBClient(attribute.ConnectionString));
    }
  }
}

