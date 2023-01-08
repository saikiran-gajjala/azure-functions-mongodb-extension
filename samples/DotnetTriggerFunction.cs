using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Functions.Extension.MongoDB;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Trigger.Samples
{
  public static class DotnetTriggerFunction
  {
    [FunctionName("Dotnet-Trigger-Function")]
    public static void Run(
        [MongoDBTrigger("%mongodb_connection_string%",
                        "%database%",
                        "%collection%",
                        watchInserts: true,
                        watchUpdates: false,
                        watchDeletes : false)] MongoDBTriggerResponseData responseData,
        ILogger log)
    {
      log.LogInformation($"Change document obtained. Reponse: {JsonConvert.SerializeObject(responseData)}");
    }
  }
}

