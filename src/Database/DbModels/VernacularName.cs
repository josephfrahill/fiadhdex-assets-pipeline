namespace Database.DbModels;

public record VernacularName
{
    public int Id { get; init; }

    public required string ColId { get; init; }

    public required string Name { get; init; }

    public string? Transliteration { get; init; }

    public string? Language { get; init; }

    public bool Preferred { get; init; }

    public string? Country { get; init; }

    public string? Area { get; init; }

    public bool Merged { get; init; }
}