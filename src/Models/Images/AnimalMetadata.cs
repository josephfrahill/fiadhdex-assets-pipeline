namespace Models.Images;

public record AnimalMetadata
{
    public required List<ImageMetadata> MetadataList { get; init; }

    public string[] ManualBlackList { get; init; } = [];
}

public record ImageMetadata
{
    public required string Url { get; init; }
    public required string Title { get; init; }
    public string? LocalFileName { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public bool PassedFilter { get; init; }
    public string? RejectReason { get; init; }
    public string? SpeciesQuery { get; init; }
}