namespace Lifedex.Constants.Exclusions;

public static class DexExclusions
{
    public static readonly string[] ExcludedOrders =
    [
        "Chiroptera", //Bats
        "Notoryctemorphia", // Marsupial Moles
        "Gymnophiona", // Worm-like amphibians
    ];

    public static readonly string[] ExcludedFamilies =
    [
        "Procellariidae", // Petrels and shearwaters
        "Hydrobatidae", // Northern storm-petrels
        "Oceanitidae", // Austral storm-petrels
        "Pelecanoididae" // Diving petrels
    ];
}