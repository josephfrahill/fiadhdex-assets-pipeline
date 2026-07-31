using System.Text.Json.Serialization;

namespace FiadhDex.Models.Abstract;

public interface IAnimalBaseData
{
    [property: JsonPropertyOrder(1)] string DexId { get; init; }

    [property: JsonPropertyOrder(2)] string Name { get; init; }

    [property: JsonPropertyOrder(6)] List<string>? OtherNames { get; init; }

    [property: JsonPropertyOrder(7)] string ScientificName { get; init; }

    [property: JsonPropertyOrder(8)] string Rank { get; init; }

    [property: JsonPropertyOrder(9)] string Genus { get; init; }

    [property: JsonPropertyOrder(10)] string Family { get; init; }

    [property: JsonPropertyOrder(11)] string Order { get; init; }

    [property: JsonPropertyOrder(12)] string Type { get; init; }
}