namespace Database.DbModels;

public record VernacularName
{
    public int Id { get; set; }
    public string CatalogueOfLifeId { get; set; } = null!;
    public string Language { get; set; } = null!;
    public string Name { get; set; } = null!;
}