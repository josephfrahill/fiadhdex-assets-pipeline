using AnimalAssetsPipeline;
using AnimalAssetsPipeline.Fetchers;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using Services;
using Services.Api;
using Services.DexCreation;
using Services.Importers.Col;
using Services.Importers.Gbif;
using System.Xml.Linq;

if (args.Length < 1)
{
    Console.WriteLine("Invalid args");
    return;
}

var solutionDirectory = Utils.GetSolutionDirectory();
var configPath = Path.Combine(solutionDirectory, "pipeline-config.json");
var executingRoot = Path.Combine(solutionDirectory, "src", "AnimalAssetsPipeline").Replace('\\', '/');
var localDexesPath = Path.Combine(executingRoot, "Dexes").Replace('\\', '/');

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => { config.AddJsonFile(configPath); })
    .ConfigureLogging(logging =>
    {
        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<PipelineConfig>(context.Configuration);
        services.AddDbContext<LifeDexDbContext>(options =>
        {
            var dbDirectory = Path.Combine(solutionDirectory, "db");
            Directory.CreateDirectory(dbDirectory);
            var dbPath = Path.Combine(dbDirectory, "lifedex.db");
            options.UseSqlite($"Data Source={dbPath}");
        });

        services.AddScoped<NameUsageImporter>();
        services.AddScoped<VernacularNameImporter>();
        services.AddScoped<ColDistributionImporter>();
        services.AddScoped<GbifAnnualOccurrenceImporter>();
        services.AddScoped<DexCreator>(provider =>
            ActivatorUtilities.CreateInstance<DexCreator>(provider, localDexesPath)
        );
        services.AddSingleton<AssetGenerator>();
        services.AddSingleton<LifeDexDataFetcher>();
        services.AddSingleton<SourceImageFetcher>();
        services.AddHttpClient();
        services.AddHttpClient<LifeDexDataFetcher>(client =>
        {
            client.BaseAddress = new Uri("https://muddy-dust-f74c.lifedex.workers.dev/");
        });
        services.AddHttpClient<WikimediaImageQuerrier>(client =>
        {
            ConfigureGlobalUserAgent(client);

            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        });
        services.AddHttpClient<WikimediaImageDownloader>(client =>
        {
            ConfigureGlobalUserAgent(client);
            client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
        });
    })
    .Build();

switch (args[0])
{
    case "0":
    {
        Console.WriteLine($"Processing input: `{args[0]}`: Db Generation.");
        using var scope = host.Services.CreateScope();

        var usageImporter = scope.ServiceProvider.GetRequiredService<NameUsageImporter>();
        await usageImporter.ImportAsync();

        var vernacularImporter = scope.ServiceProvider.GetRequiredService<VernacularNameImporter>();
        await vernacularImporter.ImportAsync();

        var distributionImporter = scope.ServiceProvider.GetRequiredService<ColDistributionImporter>();
        await distributionImporter.ImportAsync();

        var gbifOccurrenceImporter = scope.ServiceProvider.GetRequiredService<GbifAnnualOccurrenceImporter>();
        await gbifOccurrenceImporter.ImportAsync();

        Console.WriteLine("Db Generation complete.");
        break;
    }

    case "1":
    {
        Console.WriteLine($"Processing input: `{args[0]}`: Dex Creation.");
        if (args.Length < 2)
        {
            Console.WriteLine("No second arg for this flow.");
            return;
        }

        var creator = host.Services.GetRequiredService<DexCreator>();
        var dexCreationResult = await creator.CreateDex(args[1]);

        if (!dexCreationResult.Successful)
            Console.WriteLine(dexCreationResult.Message);
        break;
    }
}

var argAsInt = int.Parse(args[0]);
if (argAsInt <= 4)
{
    var generator = host.Services.GetRequiredService<AssetGenerator>();
    await generator.ExecuteFlowAsync(solutionDirectory, args, localDexesPath);
}

Console.WriteLine("Application finished.");
return;

static void ConfigureGlobalUserAgent(HttpClient client) =>
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "LifeDex-AssetPipeline/1.0 (contact: frahill.joseph@gmail.com)");