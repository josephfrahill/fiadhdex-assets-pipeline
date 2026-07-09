using Database;
using Database.Constants.ColModels;
using Database.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;
using Services.Json;
using System.Text.Json;

namespace Services.Importers;

public sealed class DistributionImporter
{
    private readonly LifeDexDbContext _dbContext;
    private readonly PipelineConfig _config;

    public DistributionImporter(
        LifeDexDbContext context,
        IOptions<PipelineConfig> options)
    {
        _dbContext = context;
        _config = options.Value;
    }

    public async Task<int> ImportAsync()
    {
        if (await _dbContext.Distributions.AnyAsync())
        {
            var allDistributionIds = _dbContext.Distributions.Select(y => y.ColId);
            var existingTaxaWithoutDistributions = _dbContext.Taxa.Where(x => !allDistributionIds.Contains(x.ColId));

            var serialised = JsonSerializer.Serialize(existingTaxaWithoutDistributions, JsonConfigSettings.Options);
            var serialisedPath = Path.Combine(Utils.GetSolutionDirectory(), "db", "no-distribution-taxa.json");
            await File.WriteAllTextAsync(serialisedPath, serialised);

            Console.WriteLine("Existing data in Distributions table, skipping.");
            return 0;
        }

        var path = Path.Combine(
            _config.ColConfig?.DirectoryPath ?? "",
            _config.ColConfig?.Distribution ?? "");

        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        Console.WriteLine($"Importing {Path.GetFileName(path)}...");

        var allAnimalsIds = _dbContext.Taxa.Select(x => x.ColId);

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
                    .Equals("text", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var area = Get(values, ColDistributionColumns.Area);

            if (string.IsNullOrWhiteSpace(area))
                continue;

            var id = Get(values, ColDistributionColumns.TaxonId);

            if (!allAnimalsIds.Contains(id))
                continue;

            var record = new Distribution
            {
                ColId = id,
                Area = area,
                EstablishmentMeans = NullIfEmpty(Get(values, ColDistributionColumns.EstablishmentMeans)),
                DegreeOfEstablishment = NullIfEmpty(Get(values, ColDistributionColumns.DegreeOfEstablishment)),
                //Merged = bool.TryParse(Get(values, ColDistributionColumns.Merged), out var merged) && merged
            };

            batch.Add(record);

            if (batch.Count < batchSize)
                continue;

            await _dbContext.Distributions.AddRangeAsync(batch);
            await _dbContext.SaveChangesAsync();

            imported += batch.Count;

            Console.WriteLine($"Imported {imported:N0} distributions...");

            batch.Clear();
            _dbContext.ChangeTracker.Clear();
        }

        if (batch.Count > 0)
        {
            await _dbContext.Distributions.AddRangeAsync(batch);
            await _dbContext.SaveChangesAsync();

            imported += batch.Count;
        }

        Console.WriteLine();
        Console.WriteLine($"Processed : {processed:N0}");
        Console.WriteLine($"Imported  : {imported:N0}");

        Console.WriteLine("Distribution data successfully parsed into Distributions table.");
        Console.WriteLine();

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