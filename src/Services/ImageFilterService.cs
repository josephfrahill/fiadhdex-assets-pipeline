using Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services;

public static class ImageFilterService
{
    private static readonly string[] Blacklist =
    [
        "skeleton", "skull", "bone", "jaw",
        "karyotype", "print", "iconographia",
        "drawing", "illustration", "museum",
        "taxidermy", "fossil", "anatomy", "diagram", "coat", "multiple", "plush", "toy", "costume", "mascot", "clothing", "clothes", "breeds",
        "x-ray", "xray", "chromosome", "dogs", "cats", "kittens", "puppies", "human", "person", "people", "girl", "boy", "man", "woman",
        ".webm"
    ];

    private static readonly string[] DogBlacklist =
    [
        "lccn", "grrenland", "Sommeraften", "Waldemar", "chimneys", "Stevenage"
    ];

    public static FilterResult IsValid(CandidateImage img, string species)
    {
        var text = $"{img.Title}".ToLowerInvariant();

        if (Blacklist.Any(text.Contains))
            return new(false, text);

        if (species.Equals("Domestic Dog") && DogBlacklist.Any(text.Contains))
        {
            return new(false, text);
        }

        if (img.Width < 1000 || img.Height < 1000)
            return new(false, "Invalid dimensions");

        return new(true, null);
    }
}
