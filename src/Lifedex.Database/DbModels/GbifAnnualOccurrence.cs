namespace Lifedex.Database.DbModels;

public record GbifAnnualOccurrence
{
    public int Id { get; init; }
    public required string ColId { get; init; }
    public required string CountryCode { get; init; }
    public required int Year { get; init; }
    public required int Occurrences { get; init; }
}