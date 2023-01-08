using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(Azure.Functions.Extension.MongoDB.MongoDBWebJobStartup))]

namespace Azure.Functions.Extension.MongoDB
{
  public class MongoDBWebJobStartup : IWebJobsStartup
  {
    public void Configure(IWebJobsBuilder builder)
    {
      builder.AddMongoDB();
    }
  }
}

