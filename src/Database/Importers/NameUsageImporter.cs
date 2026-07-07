using Database.Constants.ColModels;
using Database.Constants.Existence;
using Database.Constants.Existence.Extinct;
using Database.Constants.Flagged;
using Database.DbModels;
using Microsoft.Extensions.Options;
using Models;

namespace Database.Importers;

public sealed class NameUsageImporter
{
    private static readonly HashSet<string> SupportedTypes =
    [
        "Mammalia",
        "Aves",
        "Reptilia",
        "Amphibia",
        //"Arachnida"
    ];

    private static readonly HashSet<string> SupportedRanks =
    [
        "species",
        "subspecies",
    ];

    private readonly LifeDexDbContext _context;
    private readonly PipelineConfig _config;

    public NameUsageImporter(LifeDexDbContext context, IOptions<PipelineConfig> options)
    {
        _context = context;
        _config = options.Value;
    }

    public async Task<int> ImportAsync()
    {
        await _context.Database.EnsureDeletedAsync(); // while developing
        await _context.Database.EnsureCreatedAsync();

        var path = Path.Combine(_config.ColConfig?.DirectoryPath ?? "", _config.ColConfig?.NameUsage ?? "");

        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        Console.WriteLine($"Importing {Path.GetFileName(path)}...");

        using var reader = new StreamReader(path);

        // Read header
        var headerLine = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(headerLine))
            throw new InvalidOperationException($"{Path.GetFileName(path)} is empty.");

        var headers = headerLine.Split('\t');

        var columnLookup = headers
            .Select((name, index) => new { name, index })
            .ToDictionary(x => x.name, x => x.index);

        const int batchSize = 1000;

        var batch = new List<Taxon>(batchSize);

        var imported = 0;
        var processed = 0;

        while (true)
        {
            var line = await reader.ReadLineAsync();

            // ReadLineAsync returns null when EOF is reached
            if (line is null)
                break;

            if (string.IsNullOrEmpty(line))
                continue;

            processed++;

            var values = line.Split('\t');

            var rank = GetColumn(values, ColColumns.Rank);

            if (!SupportedRanks.Contains(rank))
                continue;

            var status = GetColumn(values, ColColumns.Status);

            if (!status.Equals("accepted", StringComparison.OrdinalIgnoreCase))
                continue;

            var type = GetColumn(values, ColColumns.Type);

            if (!SupportedTypes.Contains(type))
                continue;

            if (rank == "subspecies")
            {
                switch (type)
                {
                    case "Aves":
                        continue;

                    case "Mammalia":
                    case "Reptilia":
                    case "Amphibia":
                        break;
                }
            }

            var isExtinctString = GetColumn(values, ColColumns.Extinct);
            string? isExtinct = null;

            if (bool.TryParse(isExtinctString, out var isExtinctBool))
            {
                if (isExtinctBool)
                    continue;

                isExtinct = isExtinctString;
            }

            var genus = GetColumn(values, ColColumns.Genus);
            var family = GetColumn(values, ColColumns.Family);
            var externalExtantVerified = false;

            /*
            switch (type)
            {
                case "Mammalia":
                    if (!string.IsNullOrEmpty(genus) && ExtinctMammals.ExtinctMammalGenera.Any(x =>
                            x.Equals(genus, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    if (!string.IsNullOrEmpty(genus) && ExtantTaxa.ExtantMammalGenera.Any(x =>
                            x.Equals(genus, StringComparison.OrdinalIgnoreCase)) &&
                        string.IsNullOrEmpty(isExtinctString))
                    {
                        externalExtantVerified = true;
                    }

                    if (!string.IsNullOrEmpty(family) && ExtinctMammals.ExtinctMammalFamilies.Any(x =>
                            x.Equals(family, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    if (!string.IsNullOrEmpty(genus) && FlaggedErrors.FlaggedMammalErrors.Any(x =>
                            x.Name.Equals(genus, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    break;
                case "Reptilia":
                    if (!string.IsNullOrEmpty(genus) && ExtinctReptiles.ExtinctReptileGenera.Any(x =>
                            x.Equals(genus, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    if (!string.IsNullOrEmpty(genus) && ExtantTaxa.ExtantReptileGenera.Any(x =>
                            x.Equals(genus, StringComparison.OrdinalIgnoreCase)) &&
                        string.IsNullOrEmpty(isExtinctString))
                    {
                        externalExtantVerified = true;
                    }

                    if (!string.IsNullOrEmpty(family) && ExtinctReptiles.ExtinctReptileFamilies.Any(x =>
                            x.Equals(family, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    break;
                case "Aves":
                    if (!string.IsNullOrEmpty(genus) && ExtinctBirds.ExtinctBirdGenera.Any(x =>
                            x.Equals(genus, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    if (!string.IsNullOrEmpty(family) && ExtinctBirds.ExtinctBirdFamilies.Any(x =>
                            x.Equals(family, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    break;
                case "Amphibia":
                    if (!string.IsNullOrEmpty(genus) && ExtinctAmphibians.ExtinctAmphibianGenera.Any(x =>
                            x.Equals(genus, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    if (!string.IsNullOrEmpty(family) && ExtinctAmphibians.ExtinctAmphibianFamilies.Any(x =>
                            x.Equals(family, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    break;
            }
            */

            var species = new Taxon
            {
                ColId = GetColumn(values, ColColumns.Id),
                ScientificName = GetColumn(values, ColColumns.ScientificName),
                Rank = rank,
                Genus = genus,
                Family = family,
                Order = GetColumn(values, ColColumns.Order),
                Type = type,
                SubPhylum = GetColumn(values, ColColumns.SubPhylum),
                Phylum = GetColumn(values, ColColumns.Phylum),
                IsExtinct = isExtinct,
                ExternalExtantVerified = externalExtantVerified.ToString().ToLower(),
            };

            batch.Add(species);

            if (batch.Count < batchSize)
                continue;

            await _context.Taxa.AddRangeAsync(batch);
            await _context.SaveChangesAsync();

            imported += batch.Count;

            Console.WriteLine($"Imported {imported:N0} taxa...");

            batch.Clear();

            // Speeds up large imports
            _context.ChangeTracker.Clear();
        }

        if (batch.Count > 0)
        {
            await _context.Taxa.AddRangeAsync(batch);
            await _context.SaveChangesAsync();

            imported += batch.Count;

            _context.ChangeTracker.Clear();
        }

        Console.WriteLine();
        Console.WriteLine($"Processed : {processed:N0}");
        Console.WriteLine($"Imported  : {imported:N0}");

        return imported;

        string GetColumn(string[] values, string columnName)
        {
            if (!columnLookup.TryGetValue(columnName, out var index) || index >= values.Length)
                return string.Empty;

            return values[index];
        }
    }
}