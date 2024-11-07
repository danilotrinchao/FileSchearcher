using FileSchearcher.Core.Models;

namespace FileSchearcher.Application.Interfaces
{
    public interface IFileSearcher
    {
        List<CsvFileModel> SearchFiles(string directoryPath, List<string> searchTerms, HashSet<string> allowedExtensions);
    }
}
