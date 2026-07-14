namespace Database.DbModels;

public record Taxon
{
    public required string ColId { get; init; }
    public required string ScientificName { get; init; }
    public required string Rank { get; init; }
    public required string Genus { get; init; }
    public required string Family { get; init; }
    public required string Order { get; init; }
    public required string Type { get; init; }
    public required string SubPhylum { get; init; }
    public required string Phylum { get; init; }
    public string? IsExtinct { get; init; }
    public string? ExternalExtantVerified { get; init; }
    public List<VernacularName> VernacularNames { get; init; } = [];
    public List<ColDistribution> ColDistributions { get; init; } = [];
    public List<GbifAnnualOccurrence> GbifAnnualOccurrences { get; init; } = [];
}