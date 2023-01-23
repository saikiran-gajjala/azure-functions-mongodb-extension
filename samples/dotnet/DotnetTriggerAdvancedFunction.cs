using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Custom.Azure.Functions.Extension.MongoDB;

namespace Trigger.Samples
{
  public static class DotnetTriggerAdvancedFunction
  {
    [FunctionName("Dotnet-Trigger-Advanced-Function")]
    public static void Run(
    [MongoDBTrigger(connectionString: "%mongodb_connection_string%",
                   database:"%database%",
                   collection: "%collection%",
                   pipelineMatchStage: "%pipelineMatchStage%")] string eventData,
    ILogger log)
    {
      log.LogInformation($"Change document obtained. Reponse: {eventData}");
    }
  }
}