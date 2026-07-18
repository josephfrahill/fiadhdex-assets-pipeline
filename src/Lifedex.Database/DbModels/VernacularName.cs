namespace Lifedex.Database.DbModels;

public record VernacularName
{
    public int Id { get; init; }

    public required string ColId { get; init; }

    public required string Name { get; init; }

    public required string Transliteration { get; init; }

    public required string Language { get; init; }

    public string? Country { get; init; }

    public string? Area { get; init; }
}