using FileSchearcher.Application.Interfaces;
using FileSchearcher.Core.Models;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace FileSchearcher.Infra.Service
{
    public class FileSearcher : IFileSearcher
    {
        private static readonly Regex RepositoryPattern = new Regex(@"[a-zA-Z]{3}\d{3}", RegexOptions.Compiled);

        public List<CsvFileModel> SearchFiles(string directoryPath, List<string> searchTerms, HashSet<string> allowedExtensions)
        {
            var results = new ConcurrentBag<CsvFileModel>();
            var compiledSearchPatterns = searchTerms.Select(term => new Regex(term, RegexOptions.Compiled)).ToList();

            Parallel.ForEach(Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories), filePath =>
            {
                var extension = Path.GetExtension(filePath);
                if (!allowedExtensions.Contains(extension)) return;

                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var repository = GetRepositoryName(filePath);
                var project = GetProjectName(repository);

                SearchFile(filePath, fileName, extension, repository, project, compiledSearchPatterns, results);
            });

            return results.ToList();
        }

        private void SearchFile(string filePath, string fileName, string extension, string repository, string project, List<Regex> searchPatterns, ConcurrentBag<CsvFileModel> results)
        {
            using var reader = new StreamReader(filePath);
            string line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                foreach (var pattern in searchPatterns)
                {
                    var matches = pattern.Matches(line);
                    foreach (Match match in matches)
                    {
                        results.Add(new CsvFileModel
                        {
                            FileName = fileName,
                            FileType = extension,
                            Repository = repository,
                            Project = project,
                            FoundTerm = match.Value
                        });
                    }
                }
            }
        }

        private string GetRepositoryName(string filePath)
        {
            var match = RepositoryPattern.Match(filePath);
            return match.Success ? match.Value : "Indefinido";
        }

        private string GetProjectName(string repositoryName)
        {
            return !string.IsNullOrEmpty(repositoryName) && repositoryName.Length >= 2
                ? repositoryName.Substring(0, 2)
                : "Indefinido";
        }
    }
}
