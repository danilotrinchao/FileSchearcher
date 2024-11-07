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

            // Define os cabeçalhos das colunas
            var headers = new[] { "Objeto", "Tipo do Objeto", "Repositorio", "Projeto", "Objeto Procurado" };

            // Define o tamanho mínimo de cada coluna com base no cabeçalho e expande para um bom padding
            int[] columnWidths = headers.Select(h => h.Length + 5).ToArray();

            // Ajusta o tamanho de cada coluna com base nos dados para que seja um pouco maior que o maior valor
            foreach (var record in records)
            {
                columnWidths[0] = Math.Max(columnWidths[0], record.FileName.Length + 5); // Extra padding for readability
                columnWidths[1] = Math.Max(columnWidths[1], record.FileType.Length + 5);
                columnWidths[2] = Math.Max(columnWidths[2], record.Repository.Length + 5);
                columnWidths[3] = Math.Max(columnWidths[3], record.Project.Length + 5);
                columnWidths[4] = Math.Max(columnWidths[4], record.FoundTerm.Length + 5);
            }

            // Filtra registros duplicados por arquivo e termo encontrado
            var uniqueRecords = records
                .GroupBy(r => (r.FileName, r.FoundTerm))
                .Select(g => g.First())
                .ToList();

            // Formata e escreve o CSV
            using var writer = new StreamWriter(filePath);

            // Escreve os cabeçalhos com padding apropriado
            writer.WriteLine(
                $"{headers[0].PadRight(columnWidths[0])}, {headers[1].PadRight(columnWidths[1])}, " +
                $"{headers[2].PadRight(columnWidths[2])}, {headers[3].PadRight(columnWidths[3])}, " +
                $"{headers[4].PadRight(columnWidths[4])}"
            );

            // Escreve cada linha com padding apropriado
            foreach (var record in uniqueRecords)
            {
                writer.WriteLine(
                    $"{record.FileName.PadRight(columnWidths[0])}, {record.FileType.PadRight(columnWidths[1])}, " +
                    $"{record.Repository.PadRight(columnWidths[2])}, {record.Project.PadRight(columnWidths[3])}, " +
                    $"{record.FoundTerm.PadRight(columnWidths[4])}"
                );
            }

            Console.WriteLine($"Arquivo CSV salvo em: {filePath}");
        }

    }
}
