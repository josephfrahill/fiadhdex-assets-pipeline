using Database.Constants.ColModels;
using Database.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;

namespace Database.Importers;

public sealed class DistributionImporter
{
    private readonly LifeDexDbContext _context;
    private readonly PipelineConfig _config;

    public DistributionImporter(
        LifeDexDbContext context,
        IOptions<PipelineConfig> options)
    {
        _context = context;
        _config = options.Value;
    }

    public async Task<int> ImportAsync()
    {
        if (await _context.Distributions.AnyAsync())
        {
            Console.WriteLine("Existing data in Distributions table, skipping.");
            return 0;
        }

        var path = Path.Combine(
            _config.ColConfig?.DirectoryPath ?? "",
            _config.ColConfig?.Distribution ?? "");

        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        Console.WriteLine($"Importing {Path.GetFileName(path)}...");

        using var reader = new StreamReader(path);

        var header = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(header))
            throw new InvalidOperationException("File is empty.");

        var headers = header.Split('\t');

        var lookup = headers
            .Select((name, index) => new { name, index })
            .ToDictionary(x => x.name, x => x.index);

        const int batchSize = 5000;

        var batch = new List<Distribution>(batchSize);

        var processed = 0;
        var imported = 0;

        while (true)
        {
            var line = await reader.ReadLineAsync();

            if (line is null)
                break;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            processed++;

            var values = line.Split('\t');

            // Ignore free-text distributions
            if (!Get(values, ColDistributionColumns.Gazetteer)
                    .Equals("tdwg", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var areaId = Get(values, ColDistributionColumns.AreaId);

            if (string.IsNullOrWhiteSpace(areaId))
                continue;

            var record = new Distribution
            {
                ColId = Get(values, ColDistributionColumns.TaxonId),
                AreaId = areaId,
                EstablishmentMeans = NullIfEmpty(Get(values, ColDistributionColumns.EstablishmentMeans)),
                DegreeOfEstablishment = NullIfEmpty(Get(values, ColDistributionColumns.DegreeOfEstablishment)),
                Merged = bool.TryParse(Get(values, ColDistributionColumns.Merged), out var merged) && merged
            };

            batch.Add(record);

            if (batch.Count < batchSize)
                continue;

            await _context.Distributions.AddRangeAsync(batch);
            await _context.SaveChangesAsync();

            imported += batch.Count;

            Console.WriteLine($"Imported {imported:N0} distributions...");

            batch.Clear();
            _context.ChangeTracker.Clear();
        }

        if (batch.Count > 0)
        {
            await _context.Distributions.AddRangeAsync(batch);
            await _context.SaveChangesAsync();

            imported += batch.Count;
        }

        Console.WriteLine();
        Console.WriteLine($"Processed : {processed:N0}");
        Console.WriteLine($"Imported  : {imported:N0}");

        Console.WriteLine("Distribution data successfully parsed into Distributions table.");

        return imported;

        string Get(string[] values, string column)
        {
            if (!lookup.TryGetValue(column, out var index) || index >= values.Length)
                return string.Empty;

            return values[index];
        }

        static string? NullIfEmpty(string value)
            => string.IsNullOrWhiteSpace(value)
                ? null
                : value;
    }
}