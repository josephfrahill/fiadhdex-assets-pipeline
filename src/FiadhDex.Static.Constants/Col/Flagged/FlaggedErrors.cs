namespace FiadhDex.Static.Constants.Col.Flagged;

public static class FlaggedErrors
{
    public static readonly List<FlaggedError> FlaggedMammalErrors =
    [
        new("Trypanosoma", "Genus", "Mammalia"), //this is a parasitic protozoan genus, not a mammal at all
        new("Eimeria", "Genus", "Mammalia"), //this is a genus of parasitic apicomplexans (coccidia), not a mammal.
    ];

    // Flagged bird errors:
    // Adansonia(the plant genus for Baobab trees) and Isospora or Brueelia(which are avian parasites).
    //Flagged amphibian error Dactylosoma   — this may actually be a parasite genus (blood parasite), not amphibian — worth checking
}

public record FlaggedError(string Name, string Rank, string Type);