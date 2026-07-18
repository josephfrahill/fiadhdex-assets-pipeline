namespace Lifedex.Database.DbModels;

public record ColDistribution
{
    public int Id { get; init; }
    public required string ColId { get; init; }
    public required string Area { get; init; }
}