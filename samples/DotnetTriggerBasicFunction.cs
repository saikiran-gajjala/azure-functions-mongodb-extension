using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Functions.Extension.MongoDB;

namespace Trigger.Samples
{
  public static class DotnetTriggerBasicFunction
  {
    [FunctionName("Dotnet-Trigger-Basic-Function")]
    public static void Run(
    [MongoDBTrigger("%mongodb_connection_string%",
                    "%database%",
                    "%collection%",
                    watchInserts: false,
                    watchUpdates: true,
                    watchDeletes : false,
                    watchReplaces: false,
                    watchFields: "total" )] MongoDBTriggerEventData eventData,
    ILogger log)
    {
      log.LogInformation($"Change document obtained. Reponse: {JsonConvert.SerializeObject(eventData)}");
    }
  }
}

