namespace Database.DbModels;

public record Species
{
    public required string ColId { get; init; }
    public required string ScientificName { get; init; }
    public required string Rank { get; init; }
    public string? Genus { get; init; }
    public string? Family { get; init; }
    public string? Order { get; init; }
    public required string Type { get; init; }
    public required string Phylum { get; init; }

    //public string Kingdom { get; set; } - is probably always Animalia
    public string? IsExtinct { get; init; }
    public List<VernacularName> VernacularNames { get; init; } = [];
    public List<Distribution> Distributions { get; init; } = [];
}