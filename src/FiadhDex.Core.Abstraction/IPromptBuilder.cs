namespace FiadhDex.Core.Abstraction;

public interface IPromptBuilder
{
    string Build(string scientificName, string? commonName);
}