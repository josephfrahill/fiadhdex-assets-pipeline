using Services.Api;
using Services.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalAssetPipeline;

public static class SourceImageFetcher
{
    public static async Task RunAsync(string jsonPath)
    {
        var wiki = new WikimediaApiClient();

        var animals =
            await SpeciesJsonLoader.LoadAsync(jsonPath);

        foreach (var animal in animals)
        {
            Console.WriteLine(
                $"Searching {animal.Species}");

            var images =
                await wiki.SearchImagesAsync(
                    animal.Species);

            Console.WriteLine(
                $"Found {images.Count} images");
        }
    }
}
