using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBChangeStreamListener : IListener
  {
    private readonly ITriggeredFunctionExecutor executor;
    private readonly MongoDBTriggerContext context;
    private CancellationTokenSource cancellationTokenSource;

    public MongoDBChangeStreamListener(ITriggeredFunctionExecutor executor, MongoDBTriggerContext context)
    {
      this.executor = executor;
      this.context = context;
      this.cancellationTokenSource = new CancellationTokenSource();
    }

    public void Cancel() { }

    public void Dispose() { }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      var thread = new Thread(Watch)
      {
        IsBackground = true,
      };

      thread.Start(cancellationTokenSource.Token);
      return Task.CompletedTask;
    }

    public void Watch(object parameter)
    {
      var cancellationToken = (CancellationToken)parameter;
      this.context.MongoClient.Watch(
                                 this.context.TriggerAttribute,
                                 ExecuteAsync,
                                 cancellationToken);
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    private void ExecuteAsync(MongoDBTriggerResponseData response)
    {
      var triggerData = new TriggeredFunctionData
      {
        TriggerValue = response
      };

      var task = this.executor.TryExecuteAsync(triggerData, CancellationToken.None);
      task.Wait();
    }
  }
}

