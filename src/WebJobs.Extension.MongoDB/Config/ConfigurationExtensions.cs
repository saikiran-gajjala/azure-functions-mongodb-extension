using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;

namespace Azure.Functions.Extension.MongoDB
{
  internal static class ConfigurationExtensions
  {
    internal static string ResolveSecureSetting(this IConfiguration config, INameResolver nameResolver, string currentValue)
    {
      if (string.IsNullOrWhiteSpace(currentValue))
      {
        return currentValue;
      }

      var resolved = nameResolver.ResolveWholeString(currentValue);
      var resolvedFromConfig = config.GetConnectionStringOrSetting(resolved);
      return !string.IsNullOrEmpty(resolvedFromConfig) ? resolvedFromConfig : resolved;
    }
  }
}
