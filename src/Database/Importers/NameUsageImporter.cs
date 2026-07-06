using Database.ColModels;
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

    private readonly LifeDexDbContext _context;
    private readonly PipelineConfig _config;

    public NameUsageImporter(LifeDexDbContext context, IOptions<PipelineConfig> options)
    {
        _context = context;
        _config = options.Value;
    }

    public async Task<int> ImportAsync()
    {
        var path = Path.Combine(_config.ColConfig?.DirectoryPath ?? "", _config.ColConfig?.NameUsage ?? "");

        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        Console.WriteLine($"Importing {Path.GetFileName(path)}...");

        using var reader = new StreamReader(path);

        // Read header
        var headerLine = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(headerLine))
            throw new InvalidOperationException("NameUsage.tsv is empty.");

        var headers = headerLine.Split('\t');

        var columnLookup = headers
            .Select((name, index) => new { name, index })
            .ToDictionary(x => x.name, x => x.index);

        const int batchSize = 1000;

        var batch = new List<Species>(batchSize);

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

            if (rank.Equals("unranked", StringComparison.OrdinalIgnoreCase))
                continue;

            var status = GetColumn(values, ColColumns.Status);

            if (!status.Equals("accepted", StringComparison.OrdinalIgnoreCase))
                continue;

            var type = GetColumn(values, ColColumns.Type);

            if (!SupportedTypes.Contains(type))
                continue;

            var species = new Species
            {
                ColId = GetColumn(values, ColColumns.Id),
                ScientificName = GetColumn(values, ColColumns.ScientificName),
                Rank = rank,
                Genus = GetColumn(values, ColColumns.Genus),
                Family = GetColumn(values, ColColumns.Family),
                Order = GetColumn(values, ColColumns.Order),
                Type = type,
                Phylum = GetColumn(values, ColColumns.Phylum),
                // can be null in source
                IsExtinct = bool.TryParse(GetColumn(values, ColColumns.Extinct), out var extinct)
                    ? extinct.ToString()
                    : null
                //Kingdom = GetColumn(values, "col:kingdom"),
                // Authorship = GetColumn(values, "col:authorship"),
            };

            batch.Add(species);

            if (batch.Count < batchSize)
                continue;

            await _context.Species.AddRangeAsync(batch);
            await _context.SaveChangesAsync();

            imported += batch.Count;

            Console.WriteLine($"Imported {imported:N0} species...");

            batch.Clear();

            // Speeds up large imports
            _context.ChangeTracker.Clear();
        }

        if (batch.Count > 0)
        {
            await _context.Species.AddRangeAsync(batch);
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