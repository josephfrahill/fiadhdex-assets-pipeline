using FiadhDex.Database;
using FiadhDex.Database.DbModels;
using FiadhDex.Models;
using FiadhDex.Static.Constants.Col.MappingModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FiadhDex.Core.Concrete.Importers.Col;

public sealed class VernacularNameImporter
{
    private readonly FiadhDexDbContext _dbContext;
    private readonly PipelineConfig _config;

    public VernacularNameImporter(FiadhDexDbContext context, IOptions<PipelineConfig> options)
    {
        _dbContext = context;
        _config = options.Value;
    }

    public async Task<int> ImportAsync()
    {
        if (await _dbContext.VernacularNames.AnyAsync())
        {
            /*
            var allNameIds = _dbContext.VernacularNames.Select(y => y.ColId);
            var existingTaxaWithoutNames = _dbContext.Taxa.Where(x => !allNameIds.Contains(x.ColId));

            var serialised = JsonSerializer.Serialize(existingTaxaWithoutNames, JsonConfigSettings.Options);
            var serialisedPath = Path.Combine(Utils.GetSolutionDirectory(), "db", "no-name-taxa.json");
            await File.WriteAllTextAsync(serialisedPath, serialised);
            */

            Console.WriteLine("Existing data in VernacularNames table, skipping.");
            return 0;
        }

        var path = Path.Combine(
            _config.ColConfig?.DirectoryPath ?? "",
            _config.ColConfig?.VernacularName ?? "");

        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        Console.WriteLine($"Importing {Path.GetFileName(path)}...");

        var allTaxaIds = _dbContext.Taxa.Select(x => x.ColId);

        using var reader = new StreamReader(path);

        var header = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(header))
            throw new InvalidOperationException("File is empty.");

        var headers = header.Split('\t');

        var lookup = headers
            .Select((name, index) => new { name, index })
            .ToDictionary(x => x.name, x => x.index);

        const int batchSize = 5000;

        var batch = new List<VernacularName>(batchSize);

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

            var language = Get(values, ColVernacularNameColumns.Language);

            if (string.IsNullOrEmpty(language))
                continue;

            if (!language.Equals("eng", StringComparison.OrdinalIgnoreCase))
                continue;

            var id = Get(values, ColVernacularNameColumns.TaxonId);

            if (!allTaxaIds.Contains(id))
            {
                continue;
            }

            var record = new VernacularName
            {
                ColId = id,
                Name = Get(values, ColVernacularNameColumns.Name),
                Language = language,
                Transliteration = Get(values, ColVernacularNameColumns.Transliteration),
                Country = NullIfEmpty(Get(values, ColVernacularNameColumns.Country)),
                Area = NullIfEmpty(Get(values, ColVernacularNameColumns.Area))
            };

            batch.Add(record);

            if (batch.Count < batchSize)
                continue;

            await _dbContext.VernacularNames.AddRangeAsync(batch);
            await _dbContext.SaveChangesAsync();

            imported += batch.Count;

            Console.WriteLine($"Imported {imported:N0} names...");

            batch.Clear();
            _dbContext.ChangeTracker.Clear();
        }

        if (batch.Count > 0)
        {
            await _dbContext.VernacularNames.AddRangeAsync(batch);
            await _dbContext.SaveChangesAsync();

            imported += batch.Count;
        }

        Console.WriteLine();
        Console.WriteLine($"Processed : {processed:N0}");
        Console.WriteLine($"Imported  : {imported:N0}");
        Console.WriteLine("Vernacular data successfully parsed into Vernacular table.");
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