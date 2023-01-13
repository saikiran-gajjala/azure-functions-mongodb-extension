# Azure Functions extensions for MongoDB.

| Branch | Status |
| ------ | ------ |

![main workflow](https://github.com/saikirann73/azure-functions-mongodb-extension/actions/workflows/build.yml/badge.svg)

This repository contains MongoDb trigger extensions for the **Azure WebJobs SDK**. The communication with MongoDB is based on library **MongoDB.Driver**.
Please find C# and Javascript The following samples [here](https://github.com/Saikirann73/azure-functions-mongodb-extension/tree/main/The following samples)

## Quick Start

### Bindings

To get started using the extension in a WebJob project add reference to Hackathon.Azure.Functions.Extension.MongoDB nuget package:

The following sample azure function format listens to the MongoDB operations such as insert, update, delete and replace.

```csharp
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
```

The following sample azure function format listens to the MongoDB documents satisfying the $match condition. The following example triggers the function for all the update operations to a specific field _amount_ when the value exceeds 10000

```csharp
  public static class DotnetTriggerAdvancedFunction
  {
    [FunctionName("Dotnet-Trigger-Advanced-Function")]
    public static void Run(
    [MongoDBTrigger("%mongodb_connection_string%",
                   "%database%",
                   "%collection%",
                   "{\"$and\":[{\"operationType\":{\"$in\":[\"update\"]}},{\"updateDescription.updatedFields.amount\":{\"$exists\":true}},{\"updateDescription.updatedFields.amount\":{\"$gte\":10000}}]}"
                   )] string eventData,
    ILogger log)
    {
      log.LogInformation($"Change document obtained. Reponse: {eventData}");
    }
  }
```

## Support for CosmosDB's MongoDB API

The Trigger also supports for the CosmosDB's MongoDB API. To use it for CosmosDB check the other constructors in the [MongoDBTriggerAttribute](https://github.com/Saikirann73/azure-functions-mongodb-extension/blob/main/src/Trigger/MongoDBTriggerAttribute.cs) class.

**Note**: CosmosDB has limitation with change stream with operationType & updateDescription. Please refer the https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/change-streams?tabs=csharp#current-limitations for more information. So only insert, update and replace operations are supported.

The following sample azure function format listens to the MongoDB operations such as insert, update and replace except delete.

```csharp
  public static class DotnetTriggerBasicCosmosFunction
  {
    [FunctionName("Dotnet-Trigger-Basic-Cosmos-Function")]
    public static void Run(
    [MongoDBTrigger("%cosmosdb_connection_string%",
                   "%database%",
                   "%collection%",
                   true)] string eventData,
    ILogger log)
    {
      log.LogInformation($"Change document obtained. Reponse: {eventData}");
    }
  }
```

## Azure Functions in different programming langugages

Javascript The following sample azure function can be found [here](https://github.com/Saikirann73/azure-functions-mongodb-extension/tree/main/The following samples/javascript)
