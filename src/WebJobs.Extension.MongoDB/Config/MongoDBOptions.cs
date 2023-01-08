using Microsoft.Azure.WebJobs.Hosting;
using Newtonsoft.Json;

namespace Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Configuration for MongoDb Azure Function extension
  /// </summary>
  public class MongoDBOptions : IOptionsFormatter
  {

    public string ConnectionString { get; set; }

    public string Database { get; set; }

    public string Collection { get; set; }

    public bool WatchInserts { get; set; }

    public bool WatchUpdates { get; set; }

    public bool WatchDeletes { get; set; }

    public string Format()
    {
      var serializerSettings = new JsonSerializerSettings()
      {
        DefaultValueHandling = DefaultValueHandling.Ignore,
        Formatting = Formatting.Indented,
      };

      return JsonConvert.SerializeObject(this, serializerSettings);
    }
  }
}