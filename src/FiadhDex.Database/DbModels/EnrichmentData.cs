using FiadhDex.Models.Abstract;
using FiadhDex.Models.AnimalData;
using FiadhDex.Models.Range;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiadhDex.Database.DbModels;

public record EnrichmentData : IAiEnrichmentData
{
    public string? YoungName { get; init; }
    public GenderNames? GenderNames { get; init; }
    public required string[] PluralNames { get; init; }
    public string? CollectiveNoun { get; init; }
    public required string ConservationStatus { get; init; }
    public required string ActiveTime { get; init; }
    public int Detectability { get; init; }
    public int Photographability { get; init; }
    public required string[] Habitats { get; init; }
    public required string[] GeographicRegions { get; init; }
    public string[]? EndemicTo { get; init; }
    public required string Diet { get; init; }
    public string[]? PrimaryFoods { get; init; }
    public required string DangerLevel { get; init; }
    public bool? IsVenomous { get; init; }
    public required string DangerNotes { get; init; }
    public required DoubleRange? LifeSpanWildYears { get; init; }
    public required DoubleRange? LifeSpanCaptiveYears { get; init; }
    public required DoubleRange WeightKg { get; init; }
    public required DoubleRange LengthCm { get; init; }
    public required string Description { get; init; }
    public required string FunFact { get; init; }
    public required string[] Tags { get; init; }
    public string[]? AiNotesOrWarnings { get; init; }
}
