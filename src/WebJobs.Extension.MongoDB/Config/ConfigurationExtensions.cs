using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;

namespace Peerislands.Azure.Functions.Extension.MongoDB
{
  /// <summary>
  /// Extension Methods for IConfiguration
  /// </summary>
  internal static class ConfigurationExtensions
  {
    internal static string ResolveConfigurationSetting(this IConfiguration configuration, INameResolver nameResolver, string currentValue)
    {
      if (string.IsNullOrWhiteSpace(currentValue))
      {
        return currentValue;
      }

      var resolvedValue = nameResolver.ResolveWholeString(currentValue);
      var resolvedFromConfig = configuration.GetConnectionStringOrSetting(resolvedValue);
      return !string.IsNullOrEmpty(resolvedFromConfig) ? resolvedFromConfig : resolvedValue;
    }
  }
}
