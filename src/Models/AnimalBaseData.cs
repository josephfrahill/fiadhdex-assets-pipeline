namespace Models;

public record AnimalBaseData
{
    //public required string ColId { get; init; }
    public required string DexId { get; init; }
    public required string Name { get; init; }
    public List<string>? OtherNames { get; init; }
    public required string ScientificName { get; init; }
    public required string Rank { get; init; }
    public required string Genus { get; init; }
    public required string Family { get; init; }
    public required string Order { get; init; }
    public required string Type { get; init; }
}