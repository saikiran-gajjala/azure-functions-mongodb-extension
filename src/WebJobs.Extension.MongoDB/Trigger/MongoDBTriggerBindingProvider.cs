using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBTriggerBindingProvider : ITriggerBindingProvider
  {
    private readonly MongoDBExtensionConfigProvider configProvider;
    public MongoDBTriggerBindingProvider(MongoDBExtensionConfigProvider configProvider)
    {
      this.configProvider = configProvider;
    }

    public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
    {
      var parameter = context.Parameter;
      var attribute = parameter.GetCustomAttribute<MongoDBTriggerAttribute>(false);

      if (attribute == null)
      {
        return Task.FromResult<ITriggerBinding>(null);
      }

      if (parameter.ParameterType != typeof(MongoDBTriggerResponseData))
      {
        throw new InvalidOperationException("Invalid parameter type. Use the type MongoDBTriggerResponseData for the trigger.");
      }

      var triggerBinding = new MongoDBTriggerBindingWrapper(configProvider.CreateContext(attribute));

      return Task.FromResult<ITriggerBinding>(triggerBinding);
    }
  }
}

