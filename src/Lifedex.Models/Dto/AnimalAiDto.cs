using Lifedex.Models.AnimalData;
using Lifedex.Models.Range;

namespace Lifedex.Models.Dto;

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
    public string[]? EndemicTo { get; init; }
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
    public required string[] Tags { get; init; }
    public string[]? AiWarnings { get; init; }
}

//This describes the physical environment/ecosystem the animal lives in:
public enum Habitat
{
    Forest,
    TropicalRainforest,
    Desert,
    Grassland,
    Wetland,
    Freshwater,
    Mangrove,
    FreshwaterWetlands,
    Marine,
    Coastal,
    Urban,
    Suburban,
    Agricultural,
    Caves,
    Mountainous,
}

//This describes the physical environment/ecosystem the animal lives in:
public enum NativeRegions
{
}