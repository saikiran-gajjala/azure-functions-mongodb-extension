using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Class with extention methods for <see cref="IWebJobsBuilder"/>
  /// </summary>
  public static class MongoDBWebJobsBuilderExtensions
  {
    public static IWebJobsBuilder AddMongoDB(this IWebJobsBuilder builder)
    {
      if (builder == null)
      {
        throw new ArgumentNullException(nameof(builder));
      }


      builder.AddExtension<MongoDBExtensionConfigProvider>();
      builder.Services.AddSingleton<IMongoDBServiceFactory, MongoDBServiceFactory>();
      return builder;
    }
  }
}

