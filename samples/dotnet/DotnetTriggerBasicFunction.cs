using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Hackathon.Azure.Functions.Extension.MongoDB;

namespace Trigger.Samples
{
  public static class DotnetTriggerBasicFunction
  {
    [FunctionName("Dotnet-Trigger-Basic-Function")]
    public static void Run(
    [MongoDBTrigger("%mongodb_connection_string%",
                   "%database%",
                   "%collection%",
                   false,
                   watchInserts: true,
                   watchUpdates: true,
                   watchDeletes : true,
                   watchReplaces: true)] string eventData,
    ILogger log)
    {
      log.LogInformation($"Change document obtained. Reponse: {eventData}");
    }
  }
}