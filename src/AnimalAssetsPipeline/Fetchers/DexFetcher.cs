using Lifedex.Concrete.Json;
using Lifedex.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace AnimalAssetsPipeline.Fetchers;

public class DexFetcher
{
    private readonly HttpClient _http;
    private readonly string _dexDirPathLocal;

    public DexFetcher(HttpClient http, IOptions<PipelineConfig> options)
    {
        _http = http;
        var config = options.Value;

        var outputPathRoot = Path.Combine(config.SolutionRoot, config.Folders.Output).Replace('\\', '/');

        _dexDirPathLocal = Path.Combine(outputPathRoot, "dexes");
        Directory.CreateDirectory(_dexDirPathLocal);
    }

    public async Task<ActionResult> FetchDexAsync(string dexName)
    {
        var dexPathLocalFull = Path.Combine(_dexDirPathLocal, dexName).Replace('\\', '/');
        var dexPathCloudFull = Path.Combine("dexes", dexName).Replace('\\', '/');

        CountryDex countryDex;
        if (File.Exists(dexPathLocalFull))
        {
            countryDex = await JsonDexLoader.LoadAsync(dexPathLocalFull);
            Console.WriteLine($"Loading local dex: `{dexName}`...");
        }
        else
        {
            countryDex = await GetFromCloudAsync(dexPathCloudFull);
            SaveJsonLocally(countryDex, dexPathLocalFull);
            Console.WriteLine($"Requested dex: `{dexName}` not found, fetching from cloud...");
        }

        return new ActionResult(true)
        {
            CountryDex = countryDex
        };
    }

    private async Task<CountryDex> GetFromCloudAsync(string dexPath)
    {
        var result = await _http.GetFromJsonAsync<CountryDex>(dexPath,
            JsonConfigSettings.Options);

        return result ?? throw new JsonException($"No data returned for '{dexPath}'.");
    }

    private static void SaveJsonLocally(CountryDex countryDex, string localDexPath)
    {
        var serialised = JsonSerializer.Serialize(countryDex, JsonConfigSettings.Options);
        File.WriteAllTextAsync(localDexPath, serialised);
    }
}