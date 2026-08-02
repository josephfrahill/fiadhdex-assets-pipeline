using FiadhDex.Models.AnimalData;

namespace FiadhDex.Models;

public record CountryDexBase
{
    public int Version { get; init; } = 1;
    public required DateTime DateGenerated { get; init; }
    public required int TotalCount { get; init; }
    public required int AmphibiaCount { get; init; }
    public required int AvesCount { get; init; }
    public required int MammaliaCount { get; init; }
    public required int ReptiliaCount { get; init; }
    public required List<AnimalBaseData> Animals { get; init; } = [];
    public List<AnimalBaseData>? LowOccurrenceAnimals { get; init; } = [];
}