using Amazon.S3;
using FiadhDex.AssetsPipeline;
using FiadhDex.AssetsPipeline.Fetchers;
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

var personalEmail = string.Empty;
var dbPath = string.Empty;
const string dbFileName = "fiadhdex.db";
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => { config.AddJsonFile(configPath); })
    .ConfigureLogging(logging => logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None))
    .ConfigureServices((context, services) =>
    {
        services.Configure<PipelineConfig>(context.Configuration);
        services.AddDbContext<FiadhDexDbContext>(options =>
        {
            var dbDirectory = Path.Combine(solutionDirectory, "db");
            Directory.CreateDirectory(dbDirectory);
            dbPath = Path.Combine(dbDirectory, dbFileName);
            options.UseSqlite(
                $"Data Source={dbPath};Pooling=False");
        });

        services.AddScoped<NameUsageImporter>();
        services.AddScoped<VernacularNameImporter>();
        services.AddScoped<ColDistributionImporter>();
        services.AddScoped<GbifAnnualOccurrenceImporter>();
        services.AddSingleton<IAmazonS3>(provider =>
        {
            var configuration =
                provider.GetRequiredService<IConfiguration>();

            personalEmail =
                configuration["PERSONAL_EMAIL"]
                ?? throw new InvalidOperationException(
                    "Personal email is missing.");

            var accountId =
                configuration["R2_ACCOUNT_ID"]
                ?? throw new InvalidOperationException(
                    "R2 account ID is missing.");

            var accessKey =
                configuration["R2_ACCESS_KEY_ID"]
                ?? throw new InvalidOperationException(
                    "R2 access key is missing.");

            var secretKey =
                configuration["R2_SECRET_ACCESS_KEY"]
                ?? throw new InvalidOperationException(
                    "R2 secret key is missing.");

            var s3Config = new AmazonS3Config
            {
                ServiceURL =
                    $"https://{accountId}.r2.cloudflarestorage.com",
                Timeout = TimeSpan.FromMinutes(5),
                ConnectTimeout = TimeSpan.FromMinutes(5)
            };

            return new AmazonS3Client(
                accessKey,
                secretKey,
                s3Config);
        });

        /*
        services.AddScoped<DexCreator>(provider =>
            ActivatorUtilities.CreateInstance<DexCreator>(provider)
        );
        */
        services.AddSingleton<AssetGenerator>();
        services.AddSingleton<DbCloudBackupService>();
        services.AddSingleton<SourceImageFetcher>();
        services.AddHttpClient();
        services.AddHttpClient<DexFetcher>(client => client.BaseAddress = new Uri("https://fetch-dex.fiadhdex.workers.dev/"));
        services.AddHttpClient<WikimediaImageQuerrier>(client =>
        {
            ConfigureGlobalUserAgent(client, personalEmail);

            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        });
        services.AddHttpClient<WikimediaImageDownloader>(client =>
        {
            ConfigureGlobalUserAgent(client, personalEmail);
            client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
        });
    })
    .Build();

Console.WriteLine(
    host.Services
        .GetRequiredService<IHostEnvironment>()
        .EnvironmentName);

var argAsInt = int.Parse(args[0]);
switch (argAsInt)
{
    case 0:
    {
        Console.WriteLine($"Processing input: `{args[0]}`: Db Generation.");

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

        var dbBackupService =
            host.Services
                .GetRequiredService<DbCloudBackupService>();

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
    case >= 2:
    {
        Console.WriteLine($"Processing input: `{args[0]}`: Image downloading.");
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