namespace Lifedex.Models.AnimalData;

public record Gender
{
    public required string Male { get; init; }
    public required string Female { get; init; }
    public string? MaleYoung { get; init; }
    public string? FemaleYoung { get; init; }
    public string? MaleCastrated { get; init; }
}