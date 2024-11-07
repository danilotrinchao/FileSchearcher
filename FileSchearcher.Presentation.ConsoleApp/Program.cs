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

        Console.WriteLine("Informe o caminho do diretório:");
        var directoryPath = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            Console.WriteLine("Caminho de diretório inválido.");
            return;
        }

        var serviceProvider = ConfigureServices();
        var configurationService = serviceProvider.GetService<IConfigurationService>();
        var searchFilesCommand = serviceProvider.GetService<SearchFilesCommand>();

        if (configurationService == null || searchFilesCommand == null)
        {
            Console.WriteLine("Erro ao inicializar o serviço.");
            return;
        }

        var searchTerms = configurationService.GetSearchTerms();
        // Inicia o loading no console
        var loadingTask = ShowLoadingAsync();

        // Executa o comando de busca e exportação
        await searchFilesCommand.ExecuteAsync(directoryPath);

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

    // Remova o modificador "private" para evitar o erro CS0106
    static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Registra outros serviços no contêiner de DI
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IFileSearcher, FileSearcher>();
        services.AddSingleton<ICsvExporter, CsvExporter>();
        services.AddSingleton<SearchFilesCommand>();

        return services.BuildServiceProvider();
    }
}
