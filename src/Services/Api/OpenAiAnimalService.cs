using Lifedex.Abstraction.Api;
using Microsoft.Extensions.Options;
using Models;
using OpenAI;

namespace Services.Api;

public sealed class OpenAiAnimalService : IAnimalAiService
{
    private readonly OpenAIClient _client;

    public OpenAiAnimalService(IOptions<OpenAiOptions> options)
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

        throw new NotImplementedException();
    }
}