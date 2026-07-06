namespace Models;

public record ActionResult(bool Successful, string? Message)
{
    public List<Animal> Animals { get; init; } = [];
}