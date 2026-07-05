namespace Database.DbModels;

public record Distribution
{
    public int Id { get; set; }
    public string CatalogueOfLifeId { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string? Status { get; set; }
}