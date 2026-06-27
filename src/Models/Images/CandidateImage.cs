namespace Models.Images;

public record CandidateImage
{
    public required string Title { get; init; }
    public required string Url { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public string? Description { get; init; }
    public string? MimeType { get; init; }
}