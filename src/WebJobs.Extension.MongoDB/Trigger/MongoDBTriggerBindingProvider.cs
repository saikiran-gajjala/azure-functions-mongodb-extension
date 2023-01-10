using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Configuration;

namespace Peerislands.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// MongoDBTriggerBindingProvider class is an implementation of ITriggerBindingProvider that will be called when functions are being discovered.
  /// </summary>
  public class MongoDBTriggerBindingProvider : ITriggerBindingProvider
  {
    private readonly IConfiguration config;
    private readonly INameResolver nameResolver;
    private readonly MongoDBExtensionConfigProvider configProvider;
    public MongoDBTriggerBindingProvider(MongoDBExtensionConfigProvider configProvider, IConfiguration config, INameResolver nameResolver)
    {
      this.configProvider = configProvider;
      this.config = config;
      this.nameResolver = nameResolver;
    }

    /// <summary>
    /// Creates TriggerBinding and fetches the configuration settings for the MongoDb attribute using named resolvers.
    /// </summary>
    public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
    {
      var parameter = context.Parameter;
      var attribute = parameter.GetCustomAttribute<MongoDBTriggerAttribute>(false);

      if (attribute == null)
      {
        return Task.FromResult<ITriggerBinding>(null);
      }

      attribute = this.CreateMongoDBConfiguration(attribute);

      if (parameter.ParameterType != typeof(string))
      {
        throw new InvalidOperationException("Invalid parameter type. Use the string type for the trigger.");
      }

      var triggerBinding = new MongoDBTriggerBindingWrapper(this.configProvider.CreateContext(attribute));

      return Task.FromResult<ITriggerBinding>(triggerBinding);
    }

    private MongoDBTriggerAttribute CreateMongoDBConfiguration(MongoDBTriggerAttribute attribute)
    {
      attribute.ConnectionString = this.config.ResolveConfigurationSetting(this.nameResolver, attribute.ConnectionString);
      attribute.Database = this.config.ResolveConfigurationSetting(this.nameResolver, attribute.Database);
      attribute.Collection = this.config.ResolveConfigurationSetting(this.nameResolver, attribute.Collection);
      attribute.WatchFields = this.config.ResolveConfigurationSetting(this.nameResolver, attribute.WatchFields);
      attribute.PipelineMatchStage = this.config.ResolveConfigurationSetting(this.nameResolver, attribute.PipelineMatchStage);
      return attribute;
    }
  }
}