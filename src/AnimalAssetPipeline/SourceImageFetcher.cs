using Services.Api;
using Services.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalAssetsPipeline;

public static class SourceImageFetcher
{
    public static async Task RunAsync(string jsonPath)
    {
        var wikiApi = new WikimediaApiClient();
        var downloader = new ImageDownloader();

        var animals =
            await SpeciesJsonLoader.LoadAsync(jsonPath);

        foreach (var animal in animals)
        {
            Console.WriteLine(
                $"Searching {animal.Species}");

            var images =
                await wikiApi.SearchImagesAsync(
                    animal.Species);

            var outputDir = Path.Combine("assets", "source", animal.Id);

            Directory.CreateDirectory(outputDir);

            for (int i = 0; i < images.Count; i++)
            {
                var extension =
                    Path.GetExtension(
                        new Uri(images[i]).AbsolutePath);

                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".jpg";
                }

                var outputFile =
                    Path.Combine(
                        outputDir,
                        $"{i + 1:D2}{extension}");

                await downloader.DownloadAsync(
                    images[i],
                    outputFile);
            }


            Console.WriteLine(
                $"Found {images.Count} images");
        }
    }
}
