using FiadhDex.Models.Abstract;
using System.Text.Json.Serialization;

namespace FiadhDex.Models.AnimalData;

public record AnimalBaseData : IAnimalBaseData
{
    public required string DexId { get; init; }
    public required string ColId { get; init; }
    public string? Name { get; init; }
    public List<string>? OtherNames { get; init; }
    public required string ScientificName { get; init; }
    public required string Rank { get; init; }
    public required string Genus { get; init; }
    public required string Family { get; init; }
    public required string Order { get; init; }
    public required string Type { get; init; }
    public required int GbifOccurrenceCount { get; init; }
    public string? ColDistributionTag { get; init; }
}