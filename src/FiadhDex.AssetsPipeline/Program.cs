using Amazon.S3;
using FiadhDex.AssetsPipeline;
using FiadhDex.AssetsPipeline.Fetchers;
using FiadhDex.Core.Abstraction.Api;
using FiadhDex.Core.Concrete;
using FiadhDex.Core.Concrete.Api;
using FiadhDex.Core.Concrete.Dex;
using FiadhDex.Core.Concrete.Importers.Col;
using FiadhDex.Core.Concrete.Importers.Gbif;
using FiadhDex.Database;
using FiadhDex.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

if (args.Length < 1)
{
    Console.WriteLine("Invalid args");
    return;
}

var solutionDirectory = Utils.GetSolutionDirectory();
var configPath = Path.Combine(solutionDirectory, "pipeline-config.json");

var dbPath = string.Empty;
const string dbFileName = "fiadhdex.db";

// Passing args is standard even though not relevant for now
var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile(configPath);

if (builder.Environment.IsDevelopment())
{
    //(optional: true) // continues if user secrets are not set up
    builder.Configuration.AddUserSecrets<Program>();
}
builder.Configuration.AddEnvironmentVariables();

var configuration = builder.Configuration;
var accountId = configuration["R2_ACCOUNT_ID"]
    ?? throw new InvalidOperationException();
var accessKey = configuration["R2_ACCESS_KEY_ID"]
    ?? throw new InvalidOperationException();
var secretKey = configuration["R2_SECRET_ACCESS_KEY"]
    ?? throw new InvalidOperationException();
var personalEmail = configuration["PERSONAL_EMAIL"]
    ?? throw new InvalidOperationException();
var openAiApiKey = configuration["OPENAI_API_KEY"]
    ?? throw new InvalidOperationException();

builder.Services.Configure<PipelineConfig>(configuration);
builder.Services
    .AddSingleton<IAmazonS3>(_ =>
    {
        var s3Config = new AmazonS3Config
        {
            ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com"
        };
        return new AmazonS3Client(accessKey, secretKey, s3Config);
    })
    .AddSingleton<AssetGenerator>()
    .AddSingleton<DbCloudBackupService>()
    .AddSingleton<SourceImageFetcher>()
    .AddScoped<NameUsageImporter>()
    .AddScoped<VernacularNameImporter>()
    .AddScoped<ColDistributionImporter>()
    .AddScoped<GbifAnnualOccurrenceImporter>()
    .AddScoped<IOpenAiEnrichmentService, OpenAiEnrichmentService>();
    /*(_ =>
    {
        return new OpenAiEnrichmentService(
            new OptionsWrapper<OpenAiOptions>(new OpenAiOptions { ApiKey = openAiApiKey }),
            builder.Services.GetRequiredService<FiadhDexDbContext>(),
            host.Services.GetRequiredService<IOptions<PipelineConfig>>());
    });*/

builder.Services.AddDbContext<FiadhDexDbContext>(options =>
{
    var dbDirectory = Path.Combine(solutionDirectory, "db");
    Directory.CreateDirectory(dbDirectory);
    dbPath = Path.Combine(dbDirectory, dbFileName);
    options.UseSqlite($"Data Source={dbPath};Pooling=False");
});

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<DexFetcher>(client =>
{
    client.BaseAddress = new Uri(
        "https://fetch-dex.fiadhdex.workers.dev/");
});
/*
builder.Services.AddHttpClient<OpenAiClient>(client =>
{
    client.BaseAddress = new Uri(
        "https://fetch-dex.fiadhdex.workers.dev/");
});
*/
builder.Services.AddHttpClient<WikimediaImageQuerrier>(client =>
{
    ConfigureGlobalUserAgent(client, personalEmail);
    client.DefaultRequestHeaders.Accept.ParseAdd(
        "application/json");
});
builder.Services.AddHttpClient<WikimediaImageDownloader>(client =>
{
    ConfigureGlobalUserAgent(client, personalEmail);
    client.DefaultRequestHeaders.Accept.ParseAdd(
        "application/json");
});
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
var host = builder.Build();

var argAsInt = int.Parse(args[0]);
switch (argAsInt)
{
    case 0:
    {
        Console.WriteLine($"Processing input: `{args[0]}`: Db Generation & Backup.");
        {
            using var scope = host.Services.CreateScope();

            var usageImporter =
                scope.ServiceProvider
                    .GetRequiredService<NameUsageImporter>();

            await usageImporter.ImportAsync();

            var vernacularImporter =
                scope.ServiceProvider
                    .GetRequiredService<VernacularNameImporter>();

            await vernacularImporter.ImportAsync();

            var distributionImporter =
                scope.ServiceProvider
                    .GetRequiredService<ColDistributionImporter>();

            await distributionImporter.ImportAsync();

            var gbifOccurrenceImporter =
                scope.ServiceProvider
                    .GetRequiredService<GbifAnnualOccurrenceImporter>();

            await gbifOccurrenceImporter.ImportAsync();
        } // DbContext and other scoped services disposed here

        var dbBackupService = host.Services.GetRequiredService<DbCloudBackupService>();

        await dbBackupService.PushToCloudAsync(dbPath, dbFileName);

        Console.WriteLine("Db Generation complete.");
        break;
    }

    case 1:
    {
        Console.WriteLine($"Processing input: `{args[0]}`: Dex Creation.");
        if (args.Length < 2)
        {
            Console.WriteLine("No second arg for this flow.");
            return;
        }

        using var scope = host.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<FiadhDexDbContext>();
        var pipelineOptions = scope.ServiceProvider.GetRequiredService<IOptions<PipelineConfig>>();
        var dexFetcher = scope.ServiceProvider.GetRequiredService<DexFetcher>();

        var creator = await DexCreator.InitialiseAsync(dbContext, pipelineOptions, dexFetcher);
        var dexCreationResult = await creator.CreateCountryDexBase(args[1]);

        if (!dexCreationResult.Successful)
            Console.WriteLine(dexCreationResult.ErrorMessage);
        break;
    }

    case 2:
    {
        Console.WriteLine($"Processing input: `{args[0]}`: OpenAI Enrichment.");
        if (args.Length < 2)
        {
            Console.WriteLine("No second arg for this flow.");
            return;
        }

        using var scope = host.Services.CreateScope();
        var openAiService = scope.ServiceProvider.GetRequiredService<OpenAiEnrichmentService>();
        var enrichmentResult = await openAiService.EnrichBaseDexWithOpenAiAsync();

        if (!enrichmentResult.Successful)
            Console.WriteLine(enrichmentResult.ErrorMessage);
        break;
    }
    //case 3:
    case >= 4:
    {
        var generator = host.Services.GetRequiredService<AssetGenerator>();
        await generator.ExecuteFlowAsync(args);
        break;
    }
}

Console.WriteLine("Application finished.");
return;

static void ConfigureGlobalUserAgent(HttpClient client, string personalEmail) =>
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        $"FiadhDex-AssetPipeline/1.0 (contact: {personalEmail})");