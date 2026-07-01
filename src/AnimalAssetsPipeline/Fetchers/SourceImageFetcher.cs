using Models.Images;
using Services;
using Services.Api;
using Services.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Models;

namespace AnimalAssetsPipeline.Fetchers;

public class SourceImageFetcher
{
    private readonly WikimediaApiClient _wikiApi;
    private readonly ImageDownloader _downloader;

    public SourceImageFetcher(WikimediaApiClient wikiApi, ImageDownloader downloader)
    {
        _wikiApi = wikiApi;
        _downloader = downloader;
    }

    public async Task FetchImagesAsync(List<Animal> animals, string dexPathInResults)
    {
        foreach (var species in animals)
        {
            var candidates = await _wikiApi.GetImagesDataAsync(species.Name);

            var metadataList = new List<ImageMetadata>();
            var keptImages = new List<CandidateImage>();
            var speciesNameFormatted = species.Name.ToLowerInvariant().Replace(" ", "-");
            var outputDir = Path.Combine(dexPathInResults, string.Concat(species.Id, "-", speciesNameFormatted),
                "sourced");
            Directory.CreateDirectory(outputDir);

            foreach (var img in candidates)
            {
                // write metadata file per species
                var metadataPath = Path.Combine(outputDir, "metadata.json");
                var metadata = await ReturnOrCreateAnimalMetadata(metadataPath, metadataList);

                var result = ImageFilterService.IsValid(img, species.Name, species.Plurals, metadata.ManualBlackList,
                    outputDir);

                var fileName = Utils.SanitiseFileName(img.Title);

                metadataList.Add(new ImageMetadata
                {
                    Url = img.Url,
                    Title = img.Title,
                    Width = img.Width,
                    Height = img.Height,
                    PassedFilter = result.Passed,
                    RejectReason = result.FailReason,
                    SpeciesQuery = species.Name,
                    LocalFileName = fileName
                });

                if (result.Passed)
                {
                    keptImages.Add(img);
                }
            }

            Console.WriteLine($"{species.Name}: {keptImages.Count}/{candidates.Count} images kept");

            foreach (var img in keptImages)
            {
                //var fileName = ComputeSha256(img.Url) + ".jpg";

                var fileName = Utils.SanitiseFileName(img.Title);

                var path = Path.Combine(outputDir, fileName);

                await _downloader.DownloadAsync(img.Url, path);
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

    private static async Task<AnimalMetadata> ReturnOrCreateAnimalMetadata(string metadataPath,
        List<ImageMetadata> metadataList)
    {
        if (File.Exists(metadataPath))
        {
            var json = await File.ReadAllTextAsync(metadataPath);

            var animalMetaData = JsonSerializer.Deserialize<AnimalMetadata>(json, JsonConfigSettings.Options);

            animalMetaData = animalMetaData! with
            {
                MetadataList = metadataList
            };

            await File.WriteAllTextAsync(metadataPath,
                JsonSerializer.Serialize(animalMetaData, JsonConfigSettings.Options));

            return animalMetaData;
        }
        else
        {
            var animalMetaData = new AnimalMetadata
            {
                MetadataList = metadataList
            };

            await File.WriteAllTextAsync(metadataPath,
                JsonSerializer.Serialize(animalMetaData, JsonConfigSettings.Options));

            return animalMetaData;
        }
    }
}