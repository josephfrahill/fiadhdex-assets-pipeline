using System.Net.Http.Json;
using System.Text.Json;
using Lifedex.Concrete.Json;
using Lifedex.Models;
using Microsoft.Extensions.Options;

namespace Lifedex.Concrete.Dex;

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
        //var dexPathCloudFull = Path.Combine(dexName).Replace('\\', '/');

        CountryDex countryDex;
        if (File.Exists(dexPathLocalFull))
        {
            countryDex = await JsonDexLoader.LoadAsync(dexPathLocalFull);
            Console.WriteLine($"Loading local dex: `{dexName}`...");
        }
        else
        {
            Console.WriteLine($"Requested dex: `{dexName}` not found, fetching from cloud...");
            countryDex = await GetFromCloudAsync(dexName);
            SaveJsonLocally(countryDex, dexPathLocalFull);
            Console.WriteLine($"Successfully fetched dex: `{dexName}` from cloud.");
        }

        return new ActionResult(true)
        {
            CountryDex = countryDex
        };
    }

    private async Task<CountryDex> GetFromCloudAsync(string dexName)
    {
        var result = await _http.GetFromJsonAsync<CountryDex>(dexName,
            JsonConfigSettings.Options);

        return result ?? throw new JsonException($"No data returned for '{dexName}'.");
    }

    private static void SaveJsonLocally(CountryDex countryDex, string localDexPath)
    {
        var serialised = JsonSerializer.Serialize(countryDex, JsonConfigSettings.Options);
        File.WriteAllTextAsync(localDexPath, serialised);
    }
}