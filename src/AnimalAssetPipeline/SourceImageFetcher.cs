using Models;
using Services;
using Services.Api;
using Services.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AnimalAssetsPipeline;

public static class SourceImageFetcher
{
    public static async Task RunAsync(string jsonPath)
    {
        var http = new HttpClient();
        var wikiApi = new WikimediaApiClient(http);

        var downloader = new ImageDownloader(http, maxConcurrency: 3);

        var animals = await SpeciesJsonLoader.LoadAsync(jsonPath);

        foreach (var species in animals)
        {
            var candidates = await wikiApi.GetImagesDataAsync(species.Name);

            var metadataList = new List<ImageMetadata>();

            var keptImages = new List<CandidateImage>();

            foreach (var img in candidates)
            {
                var result = ImageFilterService.IsValid(img, species.Name);

                var fileName = ComputeSha256(img.Url) + ".jpg";

                metadataList.Add(new ImageMetadata
                {
                    Url = img.Url,
                    Title = img.Title,
                    Width = img.Width,
                    Height = img.Height,
                    PassedFilter = result.Passed,
                    RejectReason = result.Reason,
                    SpeciesQuery = species.Name,
                    LocalFileName = fileName
                });

                if (result.Passed)
                {
                    keptImages.Add(img);
                }
            }

            var filtered = keptImages;

            Console.WriteLine($"{species.Name}: {filtered.Count}/{candidates.Count} images kept");

            // write metadata file per species
            var outputDir = Path.Combine("output", species.Name);
            Directory.CreateDirectory(outputDir);

            var metadataPath = Path.Combine(outputDir, "metadata.json");

            File.WriteAllText(
                metadataPath,
                JsonSerializer.Serialize(metadataList, JsonConfigSettings.Options));

            foreach (var img in filtered)
            {
                var fileName = ComputeSha256(img.Url) + ".jpg";

                var path = Path.Combine(outputDir, fileName);

                await downloader.DownloadAsync(img.Url, path);
            }

            await Task.Delay(200);
        }
    }

    private static string ComputeSha256(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));

        return Convert.ToHexString(bytes)
            .ToLowerInvariant();
    }
}