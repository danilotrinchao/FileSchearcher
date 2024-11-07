using FileSchearcher.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FileSchearcher.Infra.Service
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<string> GetSearchTerms()
        {
            var configFilePath = "appsettings.json";
            var configData = File.ReadAllText(configFilePath);

            // Deserializando diretamente o JSON para o formato correto
            var config = JsonSerializer.Deserialize<Dictionary<string, object>>(configData);

            // Verificando se a chave "SearchTerms" existe e retornando como lista de strings
            if (config.ContainsKey("SearchTerms"))
            {
                // Obtendo o valor de "SearchTerms" como um array JSON
                var searchTerms = config["SearchTerms"] as JsonElement?;

                if (searchTerms.HasValue)
                {
                    return searchTerms.Value.EnumerateArray()
                                            .Select(x => x.GetString())
                                            .ToList();
                }
            }

            return new List<string>(); // Retorna uma lista vazia caso não encontre os termos
        }


        public (string Username, string Password) GetGitCredentials()
        {
            var username = _configuration["GitRepository:Authentication:Username"];
            var password = _configuration["GitRepository:Authentication:Password"];
            return (username, password);
        }

    }
}
