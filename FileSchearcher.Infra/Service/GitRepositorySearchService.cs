using FileSchearcher.Application.Interfaces;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSchearcher.Infra.Service
{
    public class GitRepositorySearchService
    {
        private readonly IFileSearcher _fileSearcher;
        private readonly ICsvExporter _csvExporter;
        private readonly IConfigurationService _configurationService;
        private readonly IGitServicesHandler _gitServicesHandler;

        public GitRepositorySearchService(
            IFileSearcher fileSearcher,
            ICsvExporter csvExporter,
            IConfigurationService configurationService,
            IGitServicesHandler gitServicesHandler)
        {
            _fileSearcher = fileSearcher;
            _csvExporter = csvExporter;
            _configurationService = configurationService;
            _gitServicesHandler = gitServicesHandler;
        }

        public async Task SearchAndExportFromGitRepositoryAsync(string repositoryUrl)
        {
            // Diretório temporário para clonar o repositório
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                Console.WriteLine("Clonando o repositório...");

                // Usa o GitServicesHandler para clonar o repositório
                _gitServicesHandler.CloneRepository(repositoryUrl, tempDir);

                Console.WriteLine("Repositório clonado com sucesso!");

                // Carrega os termos de busca do serviço de configuração
                var searchTerms = _configurationService.GetSearchTerms();
                if (searchTerms == null || !searchTerms.Any())
                {
                    Console.WriteLine("Nenhum termo de busca configurado.");
                    return;
                }

                // Define extensões permitidas para a busca
                var allowedExtensions = new HashSet<string> { ".cs", ".vb", ".php", ".js", ".ts", ".cpp", ".java" };

                // Realiza a busca no repositório clonado
                var results = _fileSearcher.SearchFiles(tempDir, searchTerms, allowedExtensions);

                if (!results.Any())
                {
                    Console.WriteLine("Nenhum termo encontrado nos arquivos do repositório.");
                    return;
                }

                // Caminho para salvar o arquivo CSV
                string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string csvFilePath = Path.Combine(downloadsPath, "resultado.csv");

                // Exporta para CSV
                _csvExporter.ExportToCsv(results, csvFilePath);

                Console.WriteLine($"Arquivo CSV exportado com sucesso para: {csvFilePath}");
            }
            finally
            {
                // Limpa o repositório clonado, excluindo o diretório temporário
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                    Console.WriteLine("Repositório clonado foi removido.");
                }
            }
        }
    }
}
