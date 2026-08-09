using FiadhDex.Models.AnimalData;
using FiadhDex.Models.Range;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FiadhDex.Models.Abstract;

public interface IAiEnrichmentData
{
    [JsonPropertyOrder(3)] string? YoungName { get; init; }
    [JsonPropertyOrder(4)] GenderNames? GenderNames { get; init; }
    [JsonPropertyOrder(5)] string[] PluralNames { get; init; }
    [JsonPropertyOrder(13)] string? CollectiveNoun { get; init; }
    [JsonPropertyOrder(14)] string ConservationStatus { get; init; }
    [JsonPropertyOrder(15)] string DangerLevel { get; init; }
    [JsonPropertyOrder(16)] string DangerNotes { get; init; }
    [JsonPropertyOrder(17)] bool? IsVenomous { get; init; }
    [JsonPropertyOrder(18)] string[] Habitats { get; init; }
    [JsonPropertyOrder(19)] string[] GeographicRegions { get; init; }
    [JsonPropertyOrder(20)] string[]? EndemicTo { get; init; }
    [JsonPropertyOrder(21)] int Detectability { get; init; }
    [JsonPropertyOrder(22)] int Photographability { get; init; }
    [JsonPropertyOrder(23)] string ActiveTime { get; init; }
    [JsonPropertyOrder(25)] string Diet { get; init; }
    [JsonPropertyOrder(26)] string[]? PrimaryFoods { get; init; }
    [JsonPropertyOrder(27)] DoubleRange? LifeSpanWildYears { get; init; }
    [JsonPropertyOrder(28)] DoubleRange? LifeSpanCaptiveYears { get; init; }
    [JsonPropertyOrder(29)] DoubleRange WeightKg { get; init; }
    [JsonPropertyOrder(30)] DoubleRange LengthCm { get; init; }
    [JsonPropertyOrder(31)] string Description { get; init; }
    [JsonPropertyOrder(32)] string FunFact { get; init; }
    [JsonPropertyOrder(33)] string[] Tags { get; init; }
    [JsonPropertyOrder(34)] string[]? AiNotesOrWarnings { get; init; }
}