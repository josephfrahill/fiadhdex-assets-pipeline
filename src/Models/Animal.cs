namespace Models;

public record Animal
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Species { get; init; }
    public required string Type { get; init; }
    public required string Rarity { get; init; }
    public required string Family { get; init; }
    public required string Order { get; init; }
    public Gender? Gender { get; init; }
    public required string Young { get; init; }
    public required string[] Plurals { get; init; }
    public required string CollectiveNoun { get; init; }
    public required string ConservationStatus { get; init; }
    public required string DangerLevel { get; init; }
    public required string DangerNotes { get; init; }
    public required string[] Habitat { get; init; }
    public required string Diet { get; init; }
    public required string ActiveTime { get; init; }
    public required DoubleRange LifeSpanYears { get; init; }
    public required DoubleRange WeightKg { get; init; }
    public required DoubleRange LengthCm { get; init; }
    public required string Description { get; init; }
    public required string FunFact { get; init; }
    public required string[] NativeRegions { get; init; }
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