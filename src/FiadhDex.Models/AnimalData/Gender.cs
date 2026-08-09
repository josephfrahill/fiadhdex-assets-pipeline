namespace FiadhDex.Models.AnimalData;

public record GenderNames
{
    public required string Male { get; init; }
    public required string Female { get; init; }
    public string? MaleYoung { get; init; }
    public string? FemaleYoung { get; init; }
    public string? MaleCastrated { get; init; }
}