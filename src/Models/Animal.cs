namespace Models;

public record Animal
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Species { get; init; }
    public required string Type { get; init; }
    public required string Rarity { get; init; }
}

