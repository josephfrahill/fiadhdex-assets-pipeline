using Constants.Gbif.MappingModels;
using Database;
using Database.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;

namespace Services.Importers.Gbif;

public sealed class GbifAnnualOccurrenceImporter
{
    private readonly LifeDexDbContext _dbContext;
    private readonly PipelineConfig _config;

    public GbifAnnualOccurrenceImporter(LifeDexDbContext context, IOptions<PipelineConfig> options)
    {
        _dbContext = context;
        _config = options.Value;
    }

    public async Task<int> ImportAsync()
    {
        List<string> existingDataIds = [];
        if (await _dbContext.GbifAnnualOccurrences.AnyAsync())
        {
            existingDataIds = _dbContext.GbifAnnualOccurrences.Select(x => x.ColId).ToList();
            Console.WriteLine("Existing data in GbifAnnualOccurrences table, appending.");
        }

        var path = Path.Combine(
            _config.GbifConfig?.DirectoryPath ?? "",
            _config.GbifConfig?.OccurenceDataFileName ?? "");

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

        const int batchSize = 10000;

        var batch = new List<GbifAnnualOccurrence>(batchSize);

        var processed = 0;
        var matched = 0;
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

            var acceptedTaxonKey = Get(values, GbifAnnualOccurrenceColumns.AcceptedTaxonKey);

            if (!allAnimalsIds.Contains(acceptedTaxonKey))
                continue;

            if (existingDataIds.Contains(acceptedTaxonKey))
                continue;

            matched++;

            var countryCode = Get(values, GbifAnnualOccurrenceColumns.CountryCode);

            if (string.IsNullOrWhiteSpace(countryCode))
                continue;

            if (!short.TryParse(Get(values, GbifAnnualOccurrenceColumns.Year), out var year))
                continue;

            if (!int.TryParse(Get(values, GbifAnnualOccurrenceColumns.Occurrences), out var occurrences))
                continue;

            batch.Add(new GbifAnnualOccurrence
            {
                ColId = acceptedTaxonKey,
                CountryCode = countryCode,
                Year = year,
                Occurrences = occurrences
            });

            if (batch.Count < batchSize)
                continue;

            await _dbContext.GbifAnnualOccurrences.AddRangeAsync(batch);
            await _dbContext.SaveChangesAsync();

            imported += batch.Count;

            Console.WriteLine($"Imported {imported:N0} annual occurrences...");

            batch.Clear();
            _dbContext.ChangeTracker.Clear();
        }

        if (batch.Count > 0)
        {
            await _dbContext.GbifAnnualOccurrences.AddRangeAsync(batch);
            await _dbContext.SaveChangesAsync();

            imported += batch.Count;
        }

        Console.WriteLine();
        Console.WriteLine($"Processed : {processed:N0}");
        Console.WriteLine($"Matched   : {matched:N0}");
        Console.WriteLine($"Imported  : {imported:N0}");
        Console.WriteLine();
        Console.WriteLine("GBIF annual occurrence data successfully imported.");
        Console.WriteLine();

        return imported;

        string Get(string[] values, string column)
        {
            if (!lookup.TryGetValue(column, out var index) || index >= values.Length)
                return string.Empty;

            return values[index];
        }
    }
}