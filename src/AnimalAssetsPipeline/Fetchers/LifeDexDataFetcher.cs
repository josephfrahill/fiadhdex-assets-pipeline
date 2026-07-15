using Models;
using Services.Json;
using System.Net.Http.Json;
using System.Text.Json;

namespace AnimalAssetsPipeline.Fetchers;

public class LifeDexDataFetcher
{
    private readonly HttpClient _http;

    public LifeDexDataFetcher(HttpClient http)
    {
        _http = http;
    }

    public async Task<CountryDex> FetchDataAsync(string dexName, string dexPathCloudFull, string dexPathLocalFull)
    {
        //var localDexPath = Path.Combine(localDexesPath, dexName);

        CountryDex countryDex;
        if (File.Exists(dexPathLocalFull))
        {
            countryDex = await JsonDexLoader.LoadAsync(dexPathLocalFull);
            Console.WriteLine($"Loading local dex: `{dexName}`...");
        }
        else
        {
            countryDex = await GetFromCloudAsync(dexPathCloudFull);
            //Directory.CreateDirectory(dexPathLocalFull);
            SaveJsonLocally(countryDex, dexPathLocalFull);
            Console.WriteLine($"Requested dex: `{dexName}` not found, fetching from cloud...");
        }

        return countryDex;
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