namespace Constants.Col.Flagged;

public static class FlaggedCorrections
{
    public static readonly HashSet<string> FlaggedAmphibianMismatchedType =
    [
        /*
         * Chameleo     — this is a reptile (chameleon) genus, likely a misspelling of Chamaeleo
            Hoplocephalus — this is a reptile (elapid snake) genus, not an amphibian
            Hinulia      — this is a reptile (skink) genus/synonym
            Lophognatus  — reptile (agamid lizard), likely misspelling of Lophognathus
            Odatria      — reptile (monitor lizard) genus
            Euprepes     — reptile-associated name (old skink genus, now Eutropis/Mabuya lineage)
            Mabouia      — reptile (skink), likely variant of Mabuya
            Typhlops     — reptile (blindsnake) genus
         */
    ];

    //Flagged bird errors:

    /*
     * Adansonia      — this is a plant genus (baobab trees), not a bird
        Isospora       — parasitic protozoan genus (coccidian), not a bird
        Syringophilopsis — a mite genus (feather mite), not a bird itself — parasite of birds
        Trouessartia    — also a feather mite genus, not a bird
        Brueelia        — a genus of feather lice, not a bird
            Proterothrix is a genus of feather mites belonging to the family Proctophyllodidae and subfamily Pterodectinae

            Both Maleevosaurus, Shanshanosaurus, and Coelosaurus are technically non-avian theropod dinosaurs rather
            than birds proper, even though your extended Catalogue of Life release apparently lumps them in
            (probably via the paraphyletic "dinosaur-bird" boundary being fuzzy in some taxonomies).
     */

    // Flagged mammal errors:
    // Anomalies: The list also includes taxonomic typos like Eqvus (a misspelling of the active horse genus Equus)
    // and Kangurus (an archaic, invalid synonym for Macropus, the modern kangaroo)

    //flagged reptile errors:
    // Peratherium This is extinct, but it is not a reptile. It is an extinct genus of metatherian opossum-like marsupials
}