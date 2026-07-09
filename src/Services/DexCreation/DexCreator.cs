using System.Text.Json;
using Database;
using Microsoft.Extensions.Options;
using Models;
using Services.Json;

namespace Services.DexCreation;

public class DexCreator
{
    private readonly LifeDexDbContext _dbContext;
    private readonly string _outputPath;

    public DexCreator(LifeDexDbContext context, IOptions<PipelineConfig> options)
    {
        _dbContext = context;
        var config = options.Value;
        _outputPath = Path.Combine(config.PipelineRoot, config.Folders.Output, config.Folders.Dexes);
        Directory.CreateDirectory(_outputPath);
    }

    public async Task<ActionResult> CreateDex(string country)
    {
        var countryValidated = country.ToLower().Trim();

        if (string.IsNullOrEmpty(countryValidated))
            return new ActionResult(false, $"Expected input country is empty: {country}.");

        var countryDistributionIds = _dbContext.Distributions.Where(x =>
            x.Area.ToLower().Contains(countryValidated)).Select(x => x.ColId).ToList();

        if (countryDistributionIds.Count == 0)
            return new ActionResult(false, $"No matching countries found for {countryValidated}.");

        var countryCode = GetCountryCodeFromCountry(countryValidated);

        var allSpecies = _dbContext.Taxa.Where(x => x.Rank.Equals("species")).ToList();

        var countrySpecies = allSpecies.IntersectBy(countryDistributionIds, x => x.ColId).ToList();


        //var countrySpecies = allSpecies.Where(x => countryDistributionIds.Contains(x.ColId));

        var countryDex = countrySpecies
            .Select((x, index) => new AnimalBaseData
            {
                ColId = x.ColId,
                DexId = string.Concat(countryCode, (index + 1).ToString("000")),
                VernacularNames = _dbContext.VernacularNames
                    .Where(y => y.ColId.Equals(x.ColId)).Select(z => z.Name).ToList(),
                ScientificName = x.ScientificName,
                Rank = x.Rank,
                Genus = x.Genus,
                Family = x.Family,
                Order = x.Order,
                Type = x.Type,
            })
            .ToList();

        var dexPath = Path.Combine(_outputPath, $"{countryValidated}-dex.json.");

        var json = JsonSerializer.Serialize(countryDex, JsonConfigSettings.Options);

        await File.WriteAllTextAsync(dexPath, json);
        Console.WriteLine($"Created dex for country: `{countryValidated}` in: {_outputPath}.");

        return new ActionResult(true);
    }

    private static string GetCountryCodeFromCountry(string countryValidated)
    {
        return countryValidated[..2].ToUpper();
    }
}