using Lifedex.Models.AnimalData;

namespace Lifedex.Models;

public record ActionResult(bool Successful)
{
    public string? ErrorMessage { get; init; }
    public CountryDex? CountryDex { get; init; }
    public CountryDexBase? CountryDexBase { get; init; }
    public List<Animal>? Animals { get; init; } = [];
}