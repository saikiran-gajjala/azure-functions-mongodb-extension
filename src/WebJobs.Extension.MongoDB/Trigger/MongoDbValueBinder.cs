using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Peerislands.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Value Binder class
  /// </summary>
  public class MongoDbValueBinder : IValueBinder
  {
    private object value;

    public MongoDbValueBinder(object value)
    {
      this.value = value;
    }

    public Type Type => typeof(string);

    public Task<object> GetValueAsync()
    {
      return Task.FromResult(this.value);
    }

    public Task SetValueAsync(object value, CancellationToken cancellationToken)
    {
      this.value = value;
      return Task.CompletedTask;
    }

    public string ToInvokeString()
    {
      return this.value?.ToString();
    }
  }
}

