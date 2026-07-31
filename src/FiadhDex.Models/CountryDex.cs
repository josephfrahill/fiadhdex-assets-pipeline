using FiadhDex.Models.AnimalData;

namespace FiadhDex.Models;

public record CountryDex
{
    public int Version { get; init; }
    public required DateTime DateGenerated { get; init; }
    public required int TotalCount { get; init; }
    public required int AmphibiaCount { get; init; }
    public required int AvesCount { get; init; }
    public required int MammaliaCount { get; init; }
    public required int ReptiliaCount { get; init; }
    public required List<Animal> Animals { get; init; } = [];
}