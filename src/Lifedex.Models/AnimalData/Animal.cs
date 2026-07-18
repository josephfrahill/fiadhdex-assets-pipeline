using Lifedex.Models.Abstract;
using Lifedex.Models.Dto;

namespace Lifedex.Models.AnimalData;

public record Animal : AnimalAiDto, IAnimalBaseData
{
    public required string DexId { get; init; }
    public required string Name { get; init; }
    public List<string>? OtherNames { get; init; }
    public required string ScientificName { get; init; }
    public required string Rank { get; init; }
    public required string Genus { get; init; }
    public required string Family { get; init; }
    public required string Order { get; init; }
    public required string Type { get; init; }
    public required string Rarity { get; init; }
};