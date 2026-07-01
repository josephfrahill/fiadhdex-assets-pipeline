using Models;
using Models.Images;

namespace Services;

public static class ImageFilterService
{
    private static readonly string[] Blacklist =
    [
        "skeleton", "skull", "bone", "jaw", "brain", "kidney", "testis", "fetus", "heart", "scrotal",
        "karyotype", "print", "iconographia", "drawing", "illustration",
        "museum", "taxidermy", "fossil", "anatomy", "diagram",
        "coat", "multiple", "plush", "toy", "costume", "mascot",
        "clothing", "clothes", "breeds",
        "x-ray", "xray", "chromosome",
        "human", "person", "people", "girl", "boy", "child", "man", "woman", "baby", "babies", "elder", "elders",
        "mass",
        ".webm"
    ];

    public static FilterResult IsValid(CandidateImage img, string fileName, string species, string[] plurals,
        string[] manualBlackList,
        string outputDirectory)
    {
        //var text = img.Title.ToLowerInvariant();
        var text = Utils.SanitiseFileName(img.Title);

        if (Blacklist.Any(x => text.Contains(x, StringComparison.OrdinalIgnoreCase)) ||
            manualBlackList.Any(x => text.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            return new FilterResult(false, $"Contains blocked data: {text}");
        }

        var allPlurals = plurals.Select(p => p.ToLowerInvariant()).ToArray();
        if (allPlurals.Any(text.Contains))
        {
            return new FilterResult(false, $"Contains plural: {text}");
        }

        if (img.Width < 1000 || img.Height < 1000)
            return new FilterResult(false, "Invalid dimensions");

        return DoesImageExist(img.Title, outputDirectory)
            ? new FilterResult(false, "Existing file")
            : new FilterResult(true, null);
    }

    private static bool DoesImageExist(string imgTitle, string outputDir)
    {
        var sanitisedTitle = Utils.SanitiseFileName(imgTitle);
        var imagePath = Path.Combine(outputDir, sanitisedTitle).Replace('\\', '/');
        return File.Exists(imagePath);
    }
}