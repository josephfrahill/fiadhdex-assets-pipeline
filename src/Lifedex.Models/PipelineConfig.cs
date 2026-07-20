namespace Lifedex.Models;

public record PipelineConfig
{
    public ColConfig? ColConfig { get; init; }
    public GbifConfig? GbifConfig { get; init; }
    public required DexConfig DexConfig { get; init; }
    public required AssetsConfig AssetsConfig { get; init; }
    public required string SolutionRoot { get; init; }
    public required string MetadataFileName { get; init; }
    public required Folders Folders { get; init; }
}

public record ColConfig
{
    public required string DirectoryPath { get; init; }
    public required string NameUsage { get; init; }
    public required string VernacularName { get; init; }
    public required string Distribution { get; init; }
}

public record GbifConfig
{
    public required bool IsAppendRequired { get; init; }
    public required string DirectoryPath { get; init; }
    public required string OccurenceDataFileName { get; init; }
    public required bool IsSubSpecies { get; init; }
}

public record DexConfig
{
    public required string GlobalDexName { get; init; }
    public required bool IgnoreSubspecies { get; init; }
}

public record AssetsConfig
{
    public required string WorkingDexName { get; init; }
    public required string WorkingDexOutputFolder { get; init; }
}

public record Folders
{
    public required string Output { get; init; }
    public required string Dexes { get; init; }
    public required string Assets { get; init; }
    public required string Downloaded { get; init; }
    public required string Processed { get; init; }
    public required string Results { get; init; }
}