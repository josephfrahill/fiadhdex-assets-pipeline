using Microsoft.Extensions.Options;
using Models;
using Models.Images;
using Services;
using Services.Api;
using Services.IconPipeline;
using Services.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AnimalAssetsPipeline.Fetchers;

public class SourceImageFetcher
{
    private readonly WikimediaImageQuerrier _wikiApi;
    private readonly WikimediaImageDownloader _downloader;
    private readonly PipelineConfig _config;

    public SourceImageFetcher(WikimediaImageQuerrier wikiApi, WikimediaImageDownloader downloader,
        IOptions<PipelineConfig> options)
    {
        _wikiApi = wikiApi;
        _downloader = downloader;
        _config = options.Value;
    }

    public async Task FetchImagesAsync(List<Animal> animals, string outputPathDexPath)
    {
        foreach (var species in animals)
        {
            var speciesNameFormatted = species.Name.ToLowerInvariant().Replace(" ", "-");
            var outputPathSpeciesPath = Path.Combine(outputPathDexPath,
                string.Concat(species.Id, "-", speciesNameFormatted)).Replace('\\', '/');
            Directory.CreateDirectory(outputPathSpeciesPath);
            var metadataPath = Path.Combine(outputPathSpeciesPath, _config.MetadataFileName).Replace('\\', '/');

            var outputPathSpeciesPathDownloaded =
                Path.Combine(outputPathSpeciesPath, _config.Folders.Downloaded).Replace('\\', '/');
            Directory.CreateDirectory(outputPathSpeciesPathDownloaded);

            var metadataList = new List<ImageMetadata>();
            var keptImages = new List<CandidateImage>();
            var candidates = await _wikiApi.GetImagesDataAsync(species.Name);
            foreach (var img in candidates)
            {
                var metadata = await ReturnOrCreateAnimalMetadata(metadataPath, metadataList);
                //var fileName = Path.GetFileName(new Uri(img.Url).LocalPath);
                var fileName = Utils.SanitiseFileName(img.Title);
                var result = ImageFilterService.IsValid(img, fileName, species.Name, species.Plurals,
                    metadata.ManualBlackList,
                    outputPathSpeciesPathDownloaded);

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
                var fileName = Utils.SanitiseFileName(img.Title);
                var path = Path.Combine(outputPathSpeciesPathDownloaded, fileName).Replace('\\', '/');
                await _downloader.DownloadAsync(img.Url, path);
                // remember to compress where needed
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