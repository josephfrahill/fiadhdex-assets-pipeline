using System;
using System.Collections.Generic;
using System.Text;

namespace Models;

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
