namespace Models;

public record PipelineConfig
{
    public required DexConfig DexConfig { get; init; }
    public required string PipelineRoot { get; init; }
    public required string MetadataFileName { get; init; }
    public required Folders Folders { get; init; }
}

public record DexConfig
{
    public required string DexName { get; init; }
    public required string DexPathRoot { get; init; }
}

public record Folders
{
    public required string Output { get; init; }
    public required string Downloaded { get; init; }
    public required string Processed { get; init; }
    public required string Results { get; init; }
}