using FiadhDex.Models;
using System.Text.Json;

namespace FiadhDex.Core.Concrete.Json;

public static class JsonDexLoader
{
    public static async Task<CountryDex> LoadAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<CountryDex>(json, JsonConfigSettings.Options)
               ?? throw new JsonException($"Error deserialising requested dex at path: `{path}`.");
    }
}