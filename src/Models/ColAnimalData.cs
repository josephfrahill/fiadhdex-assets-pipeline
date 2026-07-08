namespace Models;

public record ColAnimalData
{
    public required string VernacularName { get; init; }
    public required string ScientificName { get; init; }
    public required string Rank { get; init; }
    public required string Genus { get; init; }
    public required string Family { get; init; }
    public required string Order { get; init; }
    public required string Type { get; init; }
    public required string CountyCode { get; init; }
}