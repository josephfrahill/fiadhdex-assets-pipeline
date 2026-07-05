namespace Database.DbModels;

public record Species
{
    public string CatalogueOfLifeId { get; set; } = null!;
    public string ScientificName { get; set; } = null!;
    public string Rank { get; set; } = null!;
    public string? Class { get; set; }
    public string? Order { get; set; }
    public string? Family { get; set; }
    public string? Genus { get; set; }
    public List<VernacularName> VernacularNames { get; set; } = [];
    public List<Distribution> Distributions { get; set; } = [];
}