using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Functions.Extension.MongoDB;

namespace Trigger.Samples
{
  public static class DotnetTriggerAdvancedFunction
  {
    [FunctionName("Dotnet-Trigger-Advanced-Function")]
    public static void Run(
    [MongoDBTrigger("%mongodb_connection_string%",
                    "%database%",
                    "%collection%",
                    "%pipelineMatchStage%")] MongoDBTriggerEventData eventData,
    ILogger log)
    {
      log.LogInformation($"Change document obtained. Reponse: {JsonConvert.SerializeObject(eventData)}");
    }
  }
}

