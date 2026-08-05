using FiadhDex.Models.AnimalData;

namespace FiadhDex.Models;

public record CountryDexBase
{
    public required string CountryName { get; init; }
    public required string CountryCode { get; init; }
    public required string CountryFlag { get; init; }
    public int Version { get; init; } = 1;
    public string DateGenerated { get; init; } = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mmK");
    public required int TotalCount { get; init; }
    public required int AmphibiaCount { get; init; }
    public required int AvesCount { get; init; }
    public required int MammaliaCount { get; init; }
    public required int ReptiliaCount { get; init; }
    public required List<AnimalBaseData> Animals { get; init; } = [];
    public List<AnimalBaseData>? LowOccurrenceAnimals { get; init; } = [];
}