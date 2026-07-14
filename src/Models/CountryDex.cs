namespace Models;

public record CountryDex
{
    public int AnimalCount { get; init; }
    public DateTime DateGenerated { get; init; }
    public List<AnimalBaseData> Animals { get; init; } = [];
}