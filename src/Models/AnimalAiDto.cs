namespace Models;

public record AnimalAiDto
{
    public Gender? Gender { get; init; }
    public string? Young { get; init; }
    public required string[] Plurals { get; init; }
    public string? CollectiveNoun { get; init; }
    public required string ConservationStatus { get; init; }
    public required string ActiveTime { get; init; }
    public int Detectability { get; init; }
    public required string[] Habitats { get; init; }
    public required string[] NativeRegions { get; init; }
    public required string Diet { get; init; }
    public required string DangerLevel { get; init; }
    public bool? Venomous { get; init; }
    public required string DangerNotes { get; init; }
    public required DoubleRange? LifeSpanWildYears { get; init; }
    public required DoubleRange? LifeSpanCaptiveYears { get; init; }
    public required DoubleRange WeightKg { get; init; }
    public required DoubleRange LengthCm { get; init; }
    public required string Description { get; init; }
    public required string FunFact { get; init; }
    public string? Notes { get; init; }
    public required string[] Tags { get; init; }
}

public record Gender
{
    public required string Male { get; init; }
    public required string Female { get; init; }
    public string? MaleYoung { get; init; }
    public string? FemaleYoung { get; init; }
    public string? MaleCastrated { get; init; }
}

public record DoubleRange(double Min, double Max);