using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(Azure.Functions.Extension.MongoDB.MongoDBWebJobStartup))]

namespace Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Azure Function start up implementation class
  /// </summary>
  public class MongoDBWebJobStartup : IWebJobsStartup
  {
    public void Configure(IWebJobsBuilder builder)
    {
      // Adds MongoDB extensions
      builder.AddMongoDB();
    }
  }
}

