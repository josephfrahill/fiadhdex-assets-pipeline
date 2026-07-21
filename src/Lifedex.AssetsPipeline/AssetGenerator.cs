using Lifedex.AssetsPipeline.Fetchers;
using Lifedex.Concrete.Dex;
using Lifedex.Models;
using Microsoft.Extensions.Options;

namespace Lifedex.AssetsPipeline;

public class AssetGenerator
{
    private readonly PipelineConfig _config;
    private readonly DexFetcher _dexFetcher;
    private readonly SourceImageFetcher _imageFetcher;

    public AssetGenerator(IOptions<PipelineConfig> options, DexFetcher lifeDexDataFetcher,
        SourceImageFetcher imageFetcher)
    {
        _config = options.Value;
        _dexFetcher = lifeDexDataFetcher;
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