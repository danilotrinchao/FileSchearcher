using FileSchearcher.Application.Interfaces;
using System.Text.Json;

namespace FileSchearcher.Infra.Service
{
    public class ConfigurationService : IConfigurationService
    {
        public List<string> GetSearchTerms()
        {
            var configFilePath = "appsettings.json";
            var configData = File.ReadAllText(configFilePath);
            var config = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(configData);

            return config.ContainsKey("SearchTerms") ? config["SearchTerms"] : new List<string>();
        }
    }
}
