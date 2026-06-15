using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Api;

public class ImageDownloader
{
    private readonly HttpClient _httpClient;

    public ImageDownloader()
    {
        _httpClient = new();

        _httpClient.DefaultRequestHeaders.Add(
           "User-Agent",
           "AnimalAssetPipeline/1.0 (https://github.com/josephfrahill/animal-assets-pipeline)");

    }
    public async Task DownloadAsync(
        string imageUrl,
        string outputPath)
    {
        var bytes =
            await _httpClient.GetByteArrayAsync(imageUrl);

        await File.WriteAllBytesAsync(
            outputPath,
            bytes);
    }
}
