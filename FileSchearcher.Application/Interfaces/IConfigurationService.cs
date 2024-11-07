namespace FileSchearcher.Application.Interfaces
{
    public interface IConfigurationService
    {
        List<string> GetSearchTerms();
        (string Username, string Password) GetGitCredentials();
    }
}
