namespace Database.Constants.Existence.Extinct;

public static class ExtinctBirds
{
    public static readonly HashSet<string> ExtinctBirdGenera =
    [
        "Archaehierax", //        — extinct giant Australian eagle (Oligocene/Miocene)
        "Brevirostruavis", //      — extinct enantiornithine bird (Cretaceous)
        "Cherevychnavis", //       — extinct enantiornithine bird (Cretaceous)
        "Coelosaurus", //          — extinct (often classified among dinosaur/bird-adjacent fossil taxa)
        "Cryptogyps", //           — extinct Australian vulture-like raptor (Pleistocene)
        "Dasornis", //             — extinct giant pseudotooth bird (Eocene)
        "Eopachypteryx", //        — extinct enantiornithine bird (Cretaceous) — this resolves the "Eopachypterygidae" family flag from your earlier reptile/amphibian list!
        "Gigantohierax", //        — extinct large raptor (Pleistocene Cuba)
        "Gobipteryx", //           — extinct enantiornithine bird (Cretaceous, Mongolia)
        "Gouldaeornis", //         — extinct fossil bird (Australia)
        "Harpagornis", //          — extinct Haast's eagle (Holocene New Zealand)
        "Longipteryx", //          — extinct enantiornithine bird (Cretaceous)
        "Madrynornis", //           — extinct fossil flamingo relative (Miocene)
        "Maleevosaurus", //         — extinct theropod (technically a dinosaur, likely misclassified into this bird dataset)
        "Muriwaimanu", //           — extinct early penguin (Paleocene, New Zealand)
        "Nesotrochis", //            — extinct flightless rail (Caribbean, Holocene)
        "Ornimegalonyx", //          — extinct giant owl (Pleistocene Cuba)
        "Palaeocryptonyx", //        — extinct fossil galliform
        "Paraortygoides", //          — extinct fossil galliform relative
        "Paraptenodytes", //          — extinct fossil penguin (Miocene)
        "Shanshanosaurus", //         — extinct theropod dinosaur (again, likely misclassified into your bird set)
        "Talpanas", //                — extinct flightless duck relative (Hawaii)
        "Bountyphaps", // extinct pigeon (Bounty Islands, Holocene)
        "Bellulia", // extinct fossil bird (Miocene)
        "Avipeda",


        /* Autofill below. Worth checking if still valid in base release?
        "Aepyornis", // elephant birds, Madagascar — extinct ~1000 CE
        "Mullerornis", // elephant birds, Madagascar — extinct ~1000 CE
        "Jeholornis", // Early Cretaceous basal avialan — extinct
        "Lithornis", // Paleogene paleognaths — extinct
        "Sylviornis", // giant megapode-like bird, New Caledonia — extinct
        "Teratornis", // giant raptors (incl. Teratornis) — extinct
        */
    ];

    public static readonly HashSet<string> ExtinctBirdFamilies =
    [
        "Aepyornithidae", // elephant birds, Madagascar — extinct ~1000 CE
        "Jeholornithidae", // Early Cretaceous basal avialan — extinct
        "Lithornithidae", // Paleogene paleognaths — extinct
        "Sylviornithidae", // giant megapode-like bird, New Caledonia — extinct
        "Teratornithidae", // giant raptors (incl. Teratornis) — extinct
        /*
         * Accipiteridae — not a standard family name; almost certainly meant to be Accipitridae (which is also in your list)
Phasanidae — likely a typo of Phasianidae (also present)
Procelaridae — likely a typo of Procellariidae (also present)
Sylvidae and Sylviidae — both present; these are alternate treatments of the same/overlapping group (Old World warblers), not two separate valid families

        Eopachypterygidae — I don't recognize this as a valid, described bird family from any classification I know (extant or extinct).
        It doesn't match a known taxon in standard bird taxonomy
         */
    ];
}