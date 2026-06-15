using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Services.Api;

public class WikimediaApiClient
{
    // think this is bad?
    private readonly HttpClient _httpClient =
        new();

    public async Task<List<string>> SearchImagesAsync(
        string scientificName)
    {
        var url =
            $"https://commons.wikimedia.org/w/api.php" +
            $"?action=query" +
            $"&generator=search" +
            $"&gsrsearch={Uri.EscapeDataString(scientificName)}" +
            $"&gsrnamespace=6" +
            $"&prop=imageinfo" +
            $"&iiprop=url" +
            $"&format=json";

        var json =
            await _httpClient.GetStringAsync(url);

        var results = new List<string>();

        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement
                .GetProperty("query")
                .TryGetProperty("pages",
                    out var pages))
            return results;

        foreach (var page in pages.EnumerateObject())
        {
            var imageInfo =
                page.Value
                    .GetProperty("imageinfo")[0];

            results.Add(
                imageInfo
                    .GetProperty("url")
                    .GetString()!);
        }

        return results;
    }
}
