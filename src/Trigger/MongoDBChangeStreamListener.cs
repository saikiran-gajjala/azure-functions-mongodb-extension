using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;

namespace Custom.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Listener class that interacts with MongoDB and subscribes to changes to the registered documents.
  /// </summary>
  public class MongoDBChangeStreamListener : IListener
  {
    private readonly ITriggeredFunctionExecutor executor;
    private readonly MongoDBTriggerContext context;
    private readonly CancellationTokenSource cancellationTokenSource;

    public MongoDBChangeStreamListener(ITriggeredFunctionExecutor executor, MongoDBTriggerContext context)
    {
      this.executor = executor;
      this.context = context;
      this.cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// During startup azure Function invokes to start the MongoDB listener in a background thread
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
      // Using a thread instead of a task beacuse this will be long running
      var thread = new Thread(this.Watch)
      {
        IsBackground = true,
      };

      thread.Start(this.cancellationTokenSource.Token);
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      // Nothing to clean up or dispose.
      return Task.CompletedTask;
    }

    public void Cancel() { }

    public void Dispose() { }

    private void Watch(object parameter)
    {
      var cancellationToken = (CancellationToken)parameter;
      this.context.MongoClient.Watch(
                                 this.context.TriggerAttribute,
                                 ExecuteAsync,
                                 cancellationToken);
    }

    private void ExecuteAsync(string response)
    {
      var triggerData = new TriggeredFunctionData
      {
        TriggerValue = response,
      };

      var task = this.executor.TryExecuteAsync(triggerData, CancellationToken.None);
      task.Wait();
    }
  }
}