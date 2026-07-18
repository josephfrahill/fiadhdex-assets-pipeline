using Lifedex.Abstraction.Api;
using Lifedex.Models;
using Lifedex.Models.Dto;
using Microsoft.Extensions.Options;
using OpenAI;

namespace Lifedex.Concrete.Api;

public sealed class OpenAiEnrichmentService : IAnimalAiService
{
    private readonly OpenAIClient _client;

    public OpenAiEnrichmentService(IOptions<OpenAiOptions> options)
    {
        _client = new OpenAIClient(options.Value.ApiKey);
    }

    public async Task<AnimalAiDto> EnrichAsync(
        string scientificName,
        string? commonName,
        string country,
        CancellationToken cancellationToken = default)
    {
        // TODO:
        // Build prompt
        // Call GPT
        // Deserialize JSON
        // Validate
        // Return AnimalAiData

        //var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        throw new NotImplementedException();
    }
}