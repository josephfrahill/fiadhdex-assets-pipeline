using FiadhDex.Models.Abstract;
using FiadhDex.Models.AiEnrichment;
using FiadhDex.Models.Dto;
using System.Text.Json.Serialization;

namespace FiadhDex.Models.AnimalData;

public record Animal : AiEnrichmentData, IAnimalBaseData
{
    [property: JsonPropertyOrder(1)] public required string DexId { get; init; }
    [property: JsonPropertyOrder(2)] public required string Name { get; init; }
    [property: JsonPropertyOrder(6)] public List<string>? OtherNames { get; init; }
    [property: JsonPropertyOrder(7)] public required string ScientificName { get; init; }
    [property: JsonPropertyOrder(8)] public required string Rank { get; init; }
    [property: JsonPropertyOrder(9)] public required string Genus { get; init; }
    [property: JsonPropertyOrder(10)] public required string Family { get; init; }
    [property: JsonPropertyOrder(11)] public required string Order { get; init; }
    [property: JsonPropertyOrder(12)] public required string Type { get; init; }
    [property: JsonPropertyOrder(24)] public required string Rarity { get; init; }

    public Animal()
    {
        if (!string.IsNullOrEmpty(PreferedName))
        {
            Name = PreferedName;
        }

        if (OtherNames is { Count: 0 } && OtherPossibleNames is { Count: > 0 })
        {
            OtherNames = OtherPossibleNames;
        }

        if (OtherNames is { Count: > 0} && OtherPossibleNames is { Count: > 0 })
        {
            OtherNames.AddRange(OtherPossibleNames);
        }
    }
};