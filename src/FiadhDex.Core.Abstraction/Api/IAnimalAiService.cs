using FiadhDex.Models.Dto;

namespace FiadhDex.Core.Abstraction.Api;

public interface IAnimalAiService
{
    Task<AnimalAiDto> EnrichAsync(
        string scientificName,
        string? commonName,
        string country,
        CancellationToken cancellationToken = default);
}