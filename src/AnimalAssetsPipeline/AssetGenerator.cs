using AnimalAssetsPipeline.Fetchers;
using Microsoft.Extensions.Options;
using Models;

namespace AnimalAssetsPipeline;

public class AssetGenerator
{
    private readonly PipelineConfig _config;
    private readonly LifeDexDataFetcher _lifeDexDataFetcher;
    private readonly SourceImageFetcher _imageFetcher;

    public AssetGenerator(IOptions<PipelineConfig> options, LifeDexDataFetcher lifeDexDataFetcher,
        SourceImageFetcher imageFetcher)
    {
        _config = options.Value;
        _lifeDexDataFetcher = lifeDexDataFetcher;
        _imageFetcher = imageFetcher;
    }

    public async Task ExecuteFlowAsync(string solutionDirectory, string[] args)
    {
        var resourcePathRoot = _config.DexConfig.ResourcePathRoot;
        var outputPathRoot = Path.Combine(_config.PipelineRoot, _config.Folders.Output).Replace('\\', '/');
        Directory.CreateDirectory(outputPathRoot);

        var dexResult = await HandleJsonDexFetching();
        Console.WriteLine(dexResult.Message);

        var animals = dexResult.Animals;
        switch (args[0])
        {
            case "2":
                var outputPathDexPath = Path.Combine(outputPathRoot, resourcePathRoot).Replace('\\', '/');
                await _imageFetcher.FetchImagesAsync(animals, outputPathDexPath);
                break;
        }

        return;

        async Task<ActionResult> HandleJsonDexFetching()
        {
            //lifedex-data/
            var dexName = _config.DexConfig.WorkingDexName;

            var dexPathCloudFull = Path.Combine("dexes", dexName).Replace('\\', '/');
            var dexPathLocalRoot = Path.Combine(outputPathRoot, "dexes");
            Directory.CreateDirectory(dexPathLocalRoot);
            var dexPathLocalFull = Path.Combine(dexPathLocalRoot, dexName);

            var countryDex =
                await _lifeDexDataFetcher.FetchDataAsync(dexName, dexPathCloudFull, dexPathLocalFull);

            return new ActionResult(true, $"`{dexName}` successfully fetched.")
            {
                CountryDex = countryDex
            };
        }
    }
}