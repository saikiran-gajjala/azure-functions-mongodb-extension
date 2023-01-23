using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Custom.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// MongoDb Extension Config Provider class that implements <see cref="IExtensionConfigProvider"/> interface.
  /// </summary>
  public class MongoDBExtensionConfigProvider : IExtensionConfigProvider
  {
    private readonly IConfiguration config;
    private readonly IOptions<MongoDBOptions> options;
    private readonly IMongoDBServiceFactory mongoDBServiceFactory;
    private readonly INameResolver nameResolver;
    private readonly IWebJobsExtensionConfiguration<MongoDBExtensionConfigProvider> configuration;

    public MongoDBExtensionConfigProvider(IMongoDBServiceFactory mongoDBServiceFactory,
                                          IConfiguration config,
                                          IOptions<MongoDBOptions> options,
                                          IWebJobsExtensionConfiguration<MongoDBExtensionConfigProvider> configuration,
                                          INameResolver nameResolver)
    {
      this.mongoDBServiceFactory = mongoDBServiceFactory;
      this.config = config;
      this.options = options;
      this.nameResolver = nameResolver;
      this.configuration = configuration;
    }

    public void Initialize(ExtensionConfigContext context)
    {
      this.configuration.ConfigurationSection.Bind(this.options);

      var triggerBindingProvider = new MongoDBTriggerBindingProvider(this, this.config, this.nameResolver);
      context.AddBindingRule<MongoDBTriggerAttribute>()
          .BindToTrigger(triggerBindingProvider);
    }

    public MongoDBTriggerContext CreateContext(MongoDBTriggerAttribute attribute)
    {
      return new MongoDBTriggerContext(attribute, this.mongoDBServiceFactory.CreateMongoDBClient(attribute.ConnectionString, attribute.IsCosmosDB));
    }
  }
}

