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

    public async Task ExecuteFlowAsync(string solutionDirectory, string[] args, string localDexesPath)
    {
        var dexName = _config.DexConfig.WorkingDexName;
        var dexPathRoot = _config.DexConfig.DexPathRoot;
        var outputPathRoot = Path.Combine(_config.PipelineRoot, _config.Folders.Output).Replace('\\', '/');
        Directory.CreateDirectory(outputPathRoot);

        var dexResult = await HandleJsonDexFetching();
        Console.WriteLine(dexResult.Message);

        var animals = dexResult.Animals;
        switch (args[0])
        {
            case "2":
                var outputPathDexPath = Path.Combine(outputPathRoot, dexPathRoot).Replace('\\', '/');
                await _imageFetcher.FetchImagesAsync(animals, outputPathDexPath);
                break;
        }

        return;

        async Task<ActionResult> HandleJsonDexFetching()
        {
            var dexPathCloud = Path.Combine(dexPathRoot, dexName).Replace('\\', '/');
            var fetchedAnimals = await _lifeDexDataFetcher.FetchDataAsync(dexName, dexPathCloud, localDexesPath);

            return new ActionResult(true, "Dex json successfully fetched.")
            {
                Animals = fetchedAnimals
            };
        }
    }
}