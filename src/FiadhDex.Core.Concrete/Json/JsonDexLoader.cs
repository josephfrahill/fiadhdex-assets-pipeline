using FiadhDex.Models;
using System.Text.Json;

namespace FiadhDex.Core.Concrete.Json;

public static class JsonDexLoader
{
    public static async Task<T> LoadAsync<T>(string path)
    {
        var json = await File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<T>(json, JsonConfigSettings.Options)
               ?? throw new JsonException($"Error deserialising requested dex at path: `{path}`.");
    }
}