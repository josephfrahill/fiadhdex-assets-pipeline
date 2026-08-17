using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FiadhDex.Models.Enums;

[JsonConverter(typeof(JsonDescriptionEnumConverter<GeographicRegion>))]
public enum GeographicRegion
{
    [Description("Canary Islands")]
    CanaryIslands,
    Scotland,
    England,
    Ireland,
    Australia,
    [Description("New South Wales")]
    NewSouthwales,
    [Description("Northern Territory")]
    NorthernTerritory,
    [Description("South Australia")]
    SouthAustralia,
    Queensland,
    Tasmania,
    Victoria,
    [Description("Western Australia")]
    WesternAustralia,
    Azores,
    Madeira,
    Worldwide,
    [Description("Worldwide (as a pet)")]
    WorldwidePet
}
