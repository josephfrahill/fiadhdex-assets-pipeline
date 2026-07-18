namespace Models;

public interface IAnimalBaseData
{
    string DexId { get; init; }
    string Name { get; init; }
    List<string>? OtherNames { get; init; }
    string ScientificName { get; init; }
    string Rank { get; init; }
    string Genus { get; init; }
    string Family { get; init; }
    string Order { get; init; }
    string Type { get; init; }
    string Rarity { get; init; }
}