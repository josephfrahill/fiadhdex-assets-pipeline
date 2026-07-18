namespace Lifedex.Models;

public record CountryDex
{
    public required int TotalCount { get; init; }
    public required int AmphibiaCount { get; init; }
    public required int AvesCount { get; init; }
    public required int MammaliaCount { get; init; }
    public required int ReptiliaCount { get; init; }
    public required DateTime DateGenerated { get; init; }
    public required List<AnimalBaseData> Animals { get; init; } = [];
}