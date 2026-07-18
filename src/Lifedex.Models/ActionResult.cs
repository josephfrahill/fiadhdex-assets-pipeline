namespace Lifedex.Models;

public record ActionResult(bool Successful, string? Message = null)
{
    public CountryDex? CountryDex { get; init; }
    public List<Animal> Animals { get; init; } = [];
}