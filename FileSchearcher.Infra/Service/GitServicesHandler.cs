using FileSchearcher.Application.Interfaces;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;

namespace FileSchearcher.Infra.Service
{
    public class GitServicesHandler: IGitServicesHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationService _configurationService;

        public GitServicesHandler(IConfiguration configuration, IConfigurationService configurationService)
        {
            _configuration = configuration;
            _configurationService = configurationService;
        }

        public string CloneRepository(string repoUrl, string localPath)
        {
            try
            {
                // Lê as credenciais do JSON
                var user = _configurationService.GetGitCredentials();

                var cloneOptions = new CloneOptions();

                // Configura as credenciais, se fornecidas no JSON
                if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
                {
                    cloneOptions.FetchOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = user.Username,
                        Password = user.Password
                    };
                }
                
                Repository.Clone(repoUrl, localPath, cloneOptions);
                return localPath;
            }
            catch (LibGit2Sharp.LibGit2SharpException ex)
            {
                Console.WriteLine($"Erro ao clonar repositório: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.StackTrace}");
                throw;
            }
            
        }

        public void DeleteRepository(string localPath)
        {
            if (Directory.Exists(localPath))
            {
                Directory.Delete(localPath, recursive: true);
            }
        }
    }
}
