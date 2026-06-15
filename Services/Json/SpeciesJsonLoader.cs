using System.Text.Json;
using Models;

namespace Services.Json;

public static class SpeciesJsonLoader
{
    public static async Task<List<Animal>> LoadAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<List<Animal>>(json)
                ?? [];
    }
}

