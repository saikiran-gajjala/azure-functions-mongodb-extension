using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Custom.Azure.Functions.Extension.MongoDB;

namespace Trigger.Samples
{
  public static class DotnetTriggerCosmosDBFunction
  {
    [FunctionName("Dotnet-Trigger-CosmosDB-Function")]
    public static void Run(
    [MongoDBTrigger(connectionString: "%cosmosdb_connection_string%",
                   database:"%database%",
                   collection: "%collection%",
                   isCosmosDB: true)] string eventData,
    ILogger log)
    {
      log.LogInformation($"Change document obtained. Reponse: {eventData}");
    }
  }
}