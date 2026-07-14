using AnimalAssetsPipeline.Fetchers;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Services;
using Services.Api;
using Services.DexCreation;
using Services.Importers.Col;

if (args.Length < 1)
{
    Console.WriteLine("Invalid args");
    return;
}

var solutionDirectory = Utils.GetSolutionDirectory();
var configPath = Path.Combine(solutionDirectory, "pipeline-config.json");
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
        services.AddScoped<DexCreator>();
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

if (args[0].Equals("0"))
{
    Console.WriteLine($"Processing input: `{args[0]}`: Db Generation.");
    using var scope = host.Services.CreateScope();

    var usageImporter = scope.ServiceProvider.GetRequiredService<NameUsageImporter>();
    await usageImporter.ImportAsync();

    var vernacularImporter = scope.ServiceProvider.GetRequiredService<VernacularNameImporter>();
    await vernacularImporter.ImportAsync();

    var distributionImporter = scope.ServiceProvider.GetRequiredService<ColDistributionImporter>();
    await distributionImporter.ImportAsync();

    Console.WriteLine("Db Generation complete.");
    return;
}

var config = host.Services.GetRequiredService<IOptions<PipelineConfig>>().Value;
var dexName = config.DexConfig.DexName;
var dexPathRoot = config.DexConfig.DexPathRoot;
var outputPathRoot = Path.Combine(config.PipelineRoot, config.Folders.Output).Replace('\\', '/');
Directory.CreateDirectory(outputPathRoot);

var executingRoot = Path.Combine(solutionDirectory, "src", "AnimalAssetsPipeline").Replace('\\', '/');
var dataFetcher = host.Services.GetRequiredService<LifeDexDataFetcher>();
var dexResult = await HandleJsonDexFetching(dexName, dexPathRoot, executingRoot, dataFetcher);
Console.WriteLine(dexResult.Message);

var animals = dexResult.Animals;
switch (args[0])
{
    case "1":
        Console.WriteLine($"Processing input: `{args[0]}`: Dex Creation.");
        if (args.Length < 2)
        {
            Console.WriteLine("No second arg for this flow");
            return;
        }

        var creator = host.Services.GetRequiredService<DexCreator>();
        var dexCreationResult = await creator.CreateDex(args[1]);

        if (!dexCreationResult.Successful)
            Console.WriteLine(dexCreationResult.Message);
        break;

    case "2":
        var fetcher = host.Services.GetRequiredService<SourceImageFetcher>();
        var outputPathDexPath = Path.Combine(outputPathRoot, dexPathRoot).Replace('\\', '/');
        await fetcher.FetchImagesAsync(animals, outputPathDexPath);
        break;
}

return;

static async Task<ActionResult> HandleJsonDexFetching(string dexName, string dexPathRoot,
    string executingRoot, LifeDexDataFetcher dataFetcher)
{
    var localDataJsonsPath = Path.Combine(executingRoot, "DataJsons").Replace('\\', '/');

    var dexPathCloud = Path.Combine(dexPathRoot, dexName).Replace('\\', '/');
    var animals = await dataFetcher.FetchDataAsync(dexName, dexPathCloud, localDataJsonsPath);

    return new ActionResult(true, "Dex json successfully fetched.")
    {
        Animals = animals
    };
}

static void ConfigureGlobalUserAgent(HttpClient client) =>
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "LifeDex-AssetPipeline/1.0 (contact: frahill.joseph@gmail.com)");