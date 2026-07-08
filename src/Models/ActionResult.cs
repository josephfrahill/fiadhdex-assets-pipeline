namespace Models;

public record ActionResult(bool Successful, string? Message = null)
{
    public List<Animal> Animals { get; init; } = [];
}