using Lifedex.Models.AnimalData;
using Lifedex.Models.Range;

namespace Lifedex.Database.DbModels;

public class AnimalAiEnrichment
{
    public int Id { get; set; }
    public required string ColId { get; set; }
    public required string Model { get; set; }
    public required string PromptVersion { get; set; }
    public required DateTime GeneratedAt { get; set; }
    public Gender? Gender { get; init; }
    public string? Young { get; init; }
    public required string[] Plurals { get; init; }
    public string? CollectiveNoun { get; init; }
    public required string ConservationStatus { get; init; }
    public required string ActiveTime { get; init; }
    public int Detectability { get; init; }
    public int Photographability { get; init; }
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