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

    public async Task<List<Animal>> FetchDataAsync(string dexName, string cloudDexPath, string localDexesPath)
    {
        var localDexPath = Path.Combine(localDexesPath, dexName);

        List<Animal> animals;
        if (File.Exists(localDexPath))
        {
            animals = await JsonDexLoader.LoadAsync(localDexPath);
            Console.WriteLine($"Loading local dex: `{dexName}`...");
        }
        else
        {
            animals = await GetFromCloudAsync(cloudDexPath);
            Directory.CreateDirectory(localDexesPath);
            SaveJsonLocally(animals, localDexPath);
            Console.WriteLine($"Requested dex: `{dexName}` not found, fetching from cloud...");
        }

        return animals;
    }

    private async Task<List<Animal>> GetFromCloudAsync(string dexPath)
    {
        var result = await _http.GetFromJsonAsync<List<Animal>>(dexPath,
            JsonConfigSettings.Options);

        return result ?? throw new JsonException($"No data returned for '{dexPath}'.");
    }

    private static void SaveJsonLocally(List<Animal> animals, string localDexPath)
    {
        var serialised = JsonSerializer.Serialize(animals, JsonConfigSettings.Options);
        File.WriteAllTextAsync(localDexPath, serialised);
    }
}