using System.Net;

namespace Services.Api;

public class WikimediaImageDownloader
{
    private readonly HttpClient _http;
    private readonly SemaphoreSlim _throttle;

    public WikimediaImageDownloader(HttpClient http, int maxConcurrency = 3)
    {
        _http = http;
        _throttle = new SemaphoreSlim(maxConcurrency);
    }

    public async Task DownloadAsync(string url, string outputPath)
    {
        await _throttle.WaitAsync();

        try
        {
            var attempts = 0;

            while (true)
            {
                attempts++;

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await _http.SendAsync(request);

                if (response.StatusCode == (HttpStatusCode)429)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta
                                     ?? TimeSpan.FromSeconds(Math.Pow(2, attempts));

                    retryAfter += TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500));

                    await Task.Delay(retryAfter);
                    continue;
                }

                response.EnsureSuccessStatusCode();

                var bytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(outputPath, bytes);
                return;
            }
        }
        finally
        {
            _throttle.Release();
        }
    }
}