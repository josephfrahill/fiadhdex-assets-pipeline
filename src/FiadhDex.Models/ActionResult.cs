using FiadhDex.Models.AnimalData;

namespace FiadhDex.Models;

public record ActionResult(bool Successful, string? ErrorMessage = null)
{
    public CountryDex? CountryDex { get; init; }
    public CountryDexBase? CountryDexBase { get; init; }
    public List<Animal>? Animals { get; init; } = [];
}