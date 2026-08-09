using FiadhDex.Models.Abstract;
using FiadhDex.Models.AnimalData;
using FiadhDex.Models.Range;

namespace FiadhDex.Database.DbModels;

public record AiEnrichment
{
    public int Id { get; init; }
    public required string ColId { get; init; }
    public required string Model { get; init; }
    public required string PromptVersion { get; init; }
    public required string GeneratedAt { get; init; } = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mmK");
    public required string Data { get; init; }
}