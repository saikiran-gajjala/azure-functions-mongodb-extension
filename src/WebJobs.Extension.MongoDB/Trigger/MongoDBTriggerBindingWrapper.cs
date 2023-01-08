using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBTriggerBindingWrapper : ITriggerBinding
  {
    private readonly MongoDBTriggerContext triggerContext;

    public MongoDBTriggerBindingWrapper(MongoDBTriggerContext triggerContext)
    {
      this.triggerContext = triggerContext;
    }

    public Type TriggerValueType => typeof(MongoDBTriggerResponseData);

    public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>();

    public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
    {
      var valueBinder = new MongoDbValueBinder(value);
      var bindingData = new Dictionary<string, object>();
      var triggerData = new TriggerData(valueBinder, bindingData);

      return Task.FromResult<ITriggerData>(triggerData);
    }

    public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
    {
      var executor = context.Executor;
      var listener = new MongoDBChangeStreamListener(executor, triggerContext);

      return Task.FromResult<IListener>(listener);
    }

    public ParameterDescriptor ToParameterDescriptor()
    {
      return new TriggerParameterDescriptor
      {
        Name = "MongoDB",
        DisplayHints = new ParameterDisplayHints
        {
          Prompt = "MongoDB",
          Description = "MongoDB Document trigger"
        }
      };
    }
  }
}

