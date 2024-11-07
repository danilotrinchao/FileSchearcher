using FileSchearcher.Application.Interfaces;
using FileSchearcher.Core.Models;

namespace FileSchearcher.Infra.Service
{
    public class CsvExporter : ICsvExporter
    {
        public void ExportToCsv(IEnumerable<CsvFileModel> records, string outputPath = null)
        {
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string filePath = outputPath ?? Path.Combine(downloadsPath, "resultado.csv");

            using var writer = new StreamWriter(filePath);
            writer.WriteLine("Objeto,Tipo do Objeto,Repositório,Projeto,Objeto Procurado");

            foreach (var record in records)
            {
                writer.WriteLine($"{record.FileName},{record.FileType},{record.Repository},{record.Project},{record.FoundTerm}");
            }

            Console.WriteLine($"Arquivo CSV salvo em: {filePath}");
        }
    }
}
