using FileSchearcher.Application.Commands;
using FileSchearcher.Application.Interfaces;
using FileSchearcher.Infra.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Robô de Levantamento de Objetos para Migração");

        // Menu de escolha entre repositório local ou online
        Console.WriteLine("Escolha a opção de levantamento:");
        Console.WriteLine("1 - Levantamento em repositório local");
        Console.WriteLine("2 - Levantamento em repositório online");

        var option = Console.ReadLine()?.Trim();

        if (option == "1")
        {
            // Levantamento Local
            Console.WriteLine("Informe o caminho do diretório:");
            var directoryPath = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
            {
                Console.WriteLine("Caminho de diretório inválido.");
                return;
            }

            await ProcessSearch(directoryPath);
        }
        else if (option == "2")
        {
            // Levantamento Online
            Console.WriteLine("Informe a URL do repositório:");
            var repositoryUrl = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(repositoryUrl))
            {
                Console.WriteLine("URL inválida.");
                return;
            }

            var configurationService = ConfigureServices().GetService<IConfigurationService>();
            var (username, password) = configurationService?.GetGitCredentials() ?? (null, null);

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Credenciais de autenticação não encontradas.");
                return;
            }

            await ProcessSearch(repositoryUrl, username, password);
        }
        else
        {
            Console.WriteLine("Opção inválida.");
        }
    }

    private static async Task ProcessSearch(string directoryPath, string username = null, string password = null)
    {
        var serviceProvider = ConfigureServices();
        var searchFilesCommand = serviceProvider.GetService<SearchFilesCommand>();
        var configurationService = serviceProvider.GetService<IConfigurationService>();

        if (configurationService == null || searchFilesCommand == null)
        {
            Console.WriteLine("Erro ao inicializar o serviço.");
            return;
        }

        var searchTerms = configurationService.GetSearchTerms();

        // Inicia o loading no console
        var loadingTask = ShowLoadingAsync();

        if (username != null && password != null)
        {
            var gitRepositorySearchService = serviceProvider.GetService<GitRepositorySearchService>();
            await gitRepositorySearchService.SearchAndExportFromGitRepositoryAsync(directoryPath);
        }
        else
        {
            await searchFilesCommand.ExecuteAsync(directoryPath);
        }

        // Interrompe o loading quando o processo for concluído
        await loadingTask;

        Console.WriteLine("Processo concluído.");
    }

    private static async Task ShowLoadingAsync()
    {
        var loadingMessages = new[] { "Carregando", "Buscando arquivos", "Exportando para CSV" };
        int index = 0;
        while (true)
        {
            Console.Write(loadingMessages[index] + "...");
            Console.CursorLeft = 0; // Retorna o cursor ao início da linha
            index = (index + 1) % loadingMessages.Length;
            await Task.Delay(1000); // Atualiza a animação a cada 1 segundo
        }
    }

    static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Registra os serviços
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IFileSearcher, FileSearcher>();
        services.AddSingleton<ICsvExporter, CsvExporter>();
        services.AddSingleton<SearchFilesCommand>();
        services.AddSingleton<GitRepositorySearchService>();
        // Registrar a implementação da interface IGitServicesHandler
        services.AddScoped<IGitServicesHandler, GitServicesHandler>();
        // Registrar o GitRepositorySearchService
        services.AddScoped<GitRepositorySearchService>();

        return services.BuildServiceProvider();
    }
}

