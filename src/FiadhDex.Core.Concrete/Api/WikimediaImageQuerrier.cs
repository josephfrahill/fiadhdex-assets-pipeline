using FiadhDex.Models.Images;
using System.Text.Json;

namespace FiadhDex.Core.Concrete.Api;

public class WikimediaImageQuerrier
{
    private readonly HttpClient _httpClient;

    public WikimediaImageQuerrier(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CandidateImage>> GetImagesDataAsync(string scientificName)
    {
        var url =
            $"https://commons.wikimedia.org/w/api.php" +
            $"?action=query" +
            $"&generator=search" +
            $"&gsrsearch={Uri.EscapeDataString(scientificName)}" +
            $"&gsrnamespace=6" +
            $"&gsrlimit=50" +
            $"&prop=imageinfo" +
            $"&iiprop=url|size" +
            $"&format=json";

        var response = await _httpClient.GetAsync(url);

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Wikimedia API failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
        }

        using var doc = JsonDocument.Parse(body);

        var results = new List<CandidateImage>();

        if (!doc.RootElement.TryGetProperty("query", out var query) || !query.TryGetProperty("pages", out var pages))
            return results;

        foreach (var page in pages.EnumerateObject())
        {
            var imageInfoArray = page.Value.GetProperty("imageinfo");

            if (imageInfoArray.GetArrayLength() == 0)
                continue;

            var imageInfo = imageInfoArray[0];

            results.Add(new CandidateImage
            {
                Title = page.Value.GetProperty("title").GetString() ?? "",
                Url = imageInfo.GetProperty("url").GetString() ?? "",
                Width = imageInfo.GetProperty("width").GetInt32(),
                Height = imageInfo.GetProperty("height").GetInt32()
            });
        }

        return results;
    }
}