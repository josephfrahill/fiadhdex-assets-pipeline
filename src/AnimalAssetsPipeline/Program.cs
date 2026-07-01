using AnimalAssetsPipeline.Fetchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Api;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<LifeDexDataFetcher>();
        services.AddSingleton<SourceImageFetcher>();
        services.AddHttpClient();
        services.AddHttpClient<LifeDexDataFetcher>(client =>
        {
            client.BaseAddress = new Uri("https://muddy-dust-f74c.lifedex.workers.dev/");
        });
        services.AddHttpClient<WikimediaApiClient>();
        services.AddHttpClient<ImageDownloader>();
    })
    .Build();

if (args.Length < 2)
{
    Console.WriteLine("Invalid args");
    return;
}

var dataFetcher = host.Services.GetRequiredService<LifeDexDataFetcher>();

const string dexName = "global-safelist.json";
const string dexPathRoot = "safelists/global/";
const string dexPathCloud = $"{dexPathRoot}/{dexName}";


var solutionDirectory = Utils.GetSolutionDirectory();
var resultsDir = Path.Combine(solutionDirectory, "pipeline", "results");
Directory.CreateDirectory(resultsDir);
var executingRoot = Path.Combine(solutionDirectory, "src", "AnimalAssetsPipeline");
var localDataJsonsPath = Path.Combine(executingRoot, "DataJsons");

var animals = await dataFetcher.FetchDataAsync(dexName, dexPathCloud, localDataJsonsPath);

switch (args[0])
{
    case "1":
        var fetcher = host.Services.GetRequiredService<SourceImageFetcher>();
        var dexPathInResults = Path.Combine(resultsDir, dexPathRoot);
        await fetcher.FetchImagesAsync(animals, dexPathInResults);
        break;
}

static void DefensivelyCreateResultsFolders(string solutionDirectory)
{
    // var pipelineDir = Path.Combine(solutionDirectory, "pipeline");
    // Directory.CreateDirectory(pipelineDir);
    var resultsDir = Path.Combine(solutionDirectory, "pipeline", "results");
    Directory.CreateDirectory(resultsDir);
}