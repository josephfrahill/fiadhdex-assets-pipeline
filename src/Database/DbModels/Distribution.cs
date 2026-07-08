namespace Database.DbModels;

public record Distribution
{
    public int Id { get; init; }
    public required string ColId { get; init; }
    public required string AreaId { get; init; }
    public string? EstablishmentMeans { get; init; }
    public string? DegreeOfEstablishment { get; init; }
    public bool Merged { get; init; }
}