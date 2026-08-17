using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FiadhDex.Models.Enums;

[JsonConverter(typeof(JsonDescriptionEnumConverter<Habitat>))]
public enum Habitat
{
    Farms,
    Farmland,
    Forests,
    Grasslands,
    Gardens,
    Indoors,
    Stables,
    Pastures,
    Rural,
    Scrubland,
    Ports,
    Sewers,
    Riverbanks,
    [Description("Free-range paddocks")]
    FreeRangePaddocks,
    [Description("Tropical forests")]
    TropicalForests,
    [Description("Tropical rainforests")]
    TropicalRainforests,
    Deserts,    
    Freshwater,
    Mangrove,
    Ponds,
    Woodland,
    [Description("Pine forest")]
    PineForest,
    [Description("Arid regions")]
    AridRegions,
    [Description("Freshwater wetlands")]
    FreshwaterWetlands,
    [Description("Open country")]
    OpenCountry,
    Marine,
    Coastal,
    [Description("Coastal cliffs")]
    CoastalCliffs,
    Urban,
    Suburban,
    Agricultural,
    Caves,
    Mountains,
    Mountainous,
    Wetlands,
}
