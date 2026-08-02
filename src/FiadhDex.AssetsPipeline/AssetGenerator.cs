using FiadhDex.AssetsPipeline.Fetchers;
using FiadhDex.Core.Concrete.Dex;
using FiadhDex.Models;
using Microsoft.Extensions.Options;

namespace FiadhDex.AssetsPipeline;

public class AssetGenerator
{
    private readonly PipelineConfig _config;
    private readonly DexFetcher _dexFetcher;
    private readonly SourceImageFetcher _imageFetcher;

    public AssetGenerator(IOptions<PipelineConfig> options, DexFetcher dexFetcher,
        SourceImageFetcher imageFetcher)
    {
        _config = options.Value;
        _dexFetcher = dexFetcher;
        _imageFetcher = imageFetcher;

        var outputPathRoot = Path.Combine(_config.SolutionRoot, _config.Folders.Output).Replace('\\', '/');
        Directory.CreateDirectory(outputPathRoot);
    }

    public async Task ExecuteFlowAsync(string[] args)
    {
        var workingDexName = _config.AssetsConfig.WorkingDexName;

        var dexResult =
            await _dexFetcher.FetchDexAsync(workingDexName);

        if (!dexResult.Successful)
        {
            Console.WriteLine(dexResult.ErrorMessage);
        }

        var animals = dexResult.CountryDex?.Animals ?? [];

        switch (args[0])
        {
            case "2":
                await _imageFetcher.FetchImagesAsync(animals);
                break;
        }
    }
}