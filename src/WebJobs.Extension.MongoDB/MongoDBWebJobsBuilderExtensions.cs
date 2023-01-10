using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

namespace Peerislands.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Class with extention methods for <see cref="IWebJobsBuilder"/> interface.
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