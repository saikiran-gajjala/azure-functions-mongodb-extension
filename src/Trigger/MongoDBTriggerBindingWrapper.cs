using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;

namespace Hackathon.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Creates an instance of <see cref="MongoDBChangeStreamListener"/> and binds the trigger data.
  /// </summary>
  public class MongoDBTriggerBindingWrapper : ITriggerBinding
  {
    private readonly MongoDBTriggerContext triggerContext;

    public MongoDBTriggerBindingWrapper(MongoDBTriggerContext triggerContext)
    {
      this.triggerContext = triggerContext;
    }

    /// <summary>
    /// MongoDB trigger event data type string 
    /// </summary>
    public Type TriggerValueType => typeof(string);

    public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>();

    /// <summary>
    /// Azure function creates the instance of <see cref="MongoDBChangeStreamListener"/> class.
    /// </summary>
    public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
    {
      var executor = context.Executor;
      var listener = new MongoDBChangeStreamListener(executor, this.triggerContext);
      return Task.FromResult<IListener>(listener);
    }

    /// <summary>
    /// Azure function binds a value using the binding context
    /// </summary>
    public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
    {
      var valueBinder = new MongoDbValueBinder(value);
      var bindingData = new Dictionary<string, object>();
      var triggerData = new TriggerData(valueBinder, bindingData);

      return Task.FromResult<ITriggerData>(triggerData);
    }

    /// <summary>
    /// Azure function fetches the description of the binding.
    /// </summary>
    public ParameterDescriptor ToParameterDescriptor()
    {
      return new TriggerParameterDescriptor
      {
        Name = "MongoDB",
        DisplayHints = new ParameterDisplayHints
        {
          Prompt = "MongoDB",
          Description = "MongoDB Document trigger",
        },
      };
    }
  }
}