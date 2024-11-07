using FileSchearcher.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSchearcher.Application.Commands
{
    public class SearchFilesCommand
    {
        private readonly IFileSearcher _fileSearcher;
        private readonly ICsvExporter _csvExporter;
        private readonly IConfiguration _configuration;

        public SearchFilesCommand(IFileSearcher fileSearcher, ICsvExporter csvExporter, IConfiguration configuration)
        {
            _fileSearcher = fileSearcher;
            _csvExporter = csvExporter;
            _configuration = configuration;
        }

        public async Task ExecuteAsync(string directoryPath)
        {

            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
            {
                Console.WriteLine("Diretório inválido.");
                return;
            }

            // Carrega os termos de busca a partir do appsettings.json
            var searchTerms = _configuration.GetSection("SearchTerms").Get<List<string>>();
            if (searchTerms == null || searchTerms.Count() == 0)
            {
                Console.WriteLine("Nenhum termo de busca configurado.");
                return;
            }

            // Define a whitelist de extensões permitidas
            var allowedExtensions = new HashSet<string> { ".cs", ".vb", ".php", ".js", ".ts", ".cpp", ".java" };

            // Executa a busca de arquivos
            var results = _fileSearcher.SearchFiles(directoryPath, searchTerms, allowedExtensions);

            if (results.Count == 0)
            {
                Console.WriteLine("Nenhum termo encontrado nos arquivos.");
                return;
            }

            // Define o caminho para salvar o arquivo CSV na pasta Downloads
            var downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var csvFilePath = Path.Combine(downloadFolder, "resultado.csv");

            // Exporta os resultados para CSV
             _csvExporter.ExportToCsv(results, csvFilePath);

            Console.WriteLine($"A pesquisa foi concluída. Resultados exportados para: {csvFilePath}");
        }
    }
}
