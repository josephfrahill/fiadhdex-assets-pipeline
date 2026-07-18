using Lifedex.Models.Dto;

namespace Lifedex.Abstraction.Api;

public interface IAnimalAiService
{
    Task<AnimalAiDto> EnrichAsync(
        string scientificName,
        string? commonName,
        string country,
        CancellationToken cancellationToken = default);
}