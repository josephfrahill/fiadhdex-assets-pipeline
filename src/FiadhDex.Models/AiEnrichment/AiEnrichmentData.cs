using FiadhDex.Models.Abstract;
using FiadhDex.Models.AnimalData;
using FiadhDex.Models.Enums;
using FiadhDex.Models.Range;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FiadhDex.Models.AiEnrichment;

public record AiEnrichmentData : IAiEnrichmentData
{
    [JsonPropertyOrder(2)] public string? PreferedName { get; init; }
    [JsonPropertyOrder(6)] public List<string>? OtherPossibleNames { get; init; }
    [JsonPropertyOrder(3)] public string? YoungName { get; init; }
    [JsonPropertyOrder(4)] public GenderNames? GenderNames { get; init; }
    [JsonPropertyOrder(5)] public required string[] PluralNames { get; init; }
    [JsonPropertyOrder(13)] public string? CollectiveNoun { get; init; }
    [JsonPropertyOrder(14)] public required string ConservationStatus { get; init; }
    [JsonPropertyOrder(15)] public required string DangerLevel { get; init; }
    [JsonPropertyOrder(16)] public required string DangerNotes { get; init; }
    [JsonPropertyOrder(17)] public bool? IsVenomous { get; init; }
    [JsonPropertyOrder(18)] public required Habitat[] Habitats { get; init; }
    [JsonPropertyOrder(19)] public required GeographicRegion[] GeographicRegions { get; init; }
    [JsonPropertyOrder(20)] public string[]? EndemicTo { get; init; }
    [JsonPropertyOrder(21)] public required int Detectability { get; init; }
    [JsonPropertyOrder(22)] public required int Photographability { get; init; }
    [JsonPropertyOrder(23)] public required string ActiveTime { get; init; }
    [JsonPropertyOrder(25)] public required string Diet { get; init; }
    [JsonPropertyOrder(26)] public string[]? PrimaryFoods { get; init; }
    [JsonPropertyOrder(27)] public required DoubleRange? LifeSpanWildYears { get; init; }
    [JsonPropertyOrder(28)] public required DoubleRange? LifeSpanCaptiveYears { get; init; }
    [JsonPropertyOrder(29)] public required DoubleRange WeightKg { get; init; }
    [JsonPropertyOrder(30)] public required DoubleRange LengthCm { get; init; }
    [JsonPropertyOrder(31)] public required string Description { get; init; }
    [JsonPropertyOrder(32)] public required string FunFact { get; init; }
    [JsonPropertyOrder(33)] public required string[] Tags { get; init; }
    [JsonPropertyOrder(34)] public string[]? AiNotesOrWarnings { get; init; }
}