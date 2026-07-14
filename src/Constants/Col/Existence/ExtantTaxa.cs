namespace Constants.Col.Existence;

public static class ExtantTaxa
{
    // Confirmed via Claude code
    public static readonly HashSet<string> ExtantReptileGenera =
    [
        "Deinagkistrodon", // (extant viper)
        "Plica", // (extant iguanid)
        "Rafetus", // (extant softshell turtle)
        "Anoplohydrus" // (extant snake)
    ];

    public static readonly HashSet<string> ExtantMammalGenera =
    [
        "Ammotragus",
        "Apnoctomys",
        "Chimaerodipus",
        "Diplomesodon",
        "Eospalax",
        "Eozapus",
        "Episoriculus",
        "Glirulus",
        "Gracilimus",
        "Gyldenstolpia",
        "Nanonycteris",
        "Neodon",
        "Peroryctes",
        "Proedromys",
        "Prometheomys",
        "Sivaonyx"
    ];

    public static readonly HashSet<string> ExtantAmphibianGenera =
    [
        /*
         * Adelophrnne     → Adelophryne
            Ceralophrys     → Ceratophrys (also separately listed correctly)
            Dryadobates     → possibly Dryophytes or a garbled entry
            Euronecturus    → possibly Necturus variant
            Guntheri        → not a genus name as such, likely a species epithet mistaken for genus
            Keneuxia        → unclear, possibly garbled
            Laceria         → likely Lacerta (reptile) or garbled amphibian name
            Lepiodactylus   → Leptodactylus
            Leptodaciylus   → Leptodactylus (duplicate typo)
            Nymphophidium   → unclear, possibly garbled
            Ptygoderus      → unclear, possibly Ptychadena garbled
            Regenia         → unclear
            Strophura       → unclear
            Tropidolepisma  → unclear, possibly garbled
            Wjayarana       → likely Raorchestes-relative typo, unclear
            Ymboirana       → unclear
         */
    ];
}