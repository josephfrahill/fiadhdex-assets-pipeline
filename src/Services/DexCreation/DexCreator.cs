using Constants.Countries;
using Database;
using Microsoft.Extensions.Options;
using Models;
using Services.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ActionResult> CreateDex(string givenCountry)
    {
        var countryValidated = givenCountry.ToLower().Trim();

        if (string.IsNullOrEmpty(countryValidated))
            return new ActionResult(false, $"Expected input country is empty: {givenCountry}.");

        if (CountryLookup.TryParse(givenCountry, out var countryData))
        {
            Console.WriteLine($"Found: {countryData.Name} [{countryData.Code}]");
        }

        /*
        var countryDistributionIds = _dbContext.ColDistributions.Where(x =>
            x.Area.Contains(countryData.Name) == true).Select(x => x.ColId).ToList();

        if (countryDistributionIds.Count == 0)
            return new ActionResult(false, $"No matching countries found for {countryData.Name}.");
        */

        var countryOccurrencesIds = _dbContext.GbifAnnualOccurrences.Where(x =>
            x.CountryCode.Equals(countryData.Code)).Select(x => x.ColId);

        if (!countryOccurrencesIds.Any())
            return new ActionResult(false, $"No matching countries found for {countryData.Name}.");

        var allSpecies = _dbContext.Taxa.Where(x => x.Rank.Equals("species")).Include(taxon => taxon.VernacularNames)
            .ToList();

        var countrySpecies = allSpecies.IntersectBy(countryOccurrencesIds, x => x.ColId).ToList();

        var animals = countrySpecies
            .Select((x, index) =>
            {
                var name = x.VernacularNames.FirstOrDefault()?.Name ?? string.Empty;
                var otherNames = x.VernacularNames.Select(y => y.Name).Except([name]).ToList();

                var animal = new AnimalBaseData
                {
                    DexId = string.Concat(countryData.Code, (index + 1).ToString("000")),
                    Name = name,
                    OtherNames = otherNames,
                    ScientificName = x.ScientificName,
                    Rank = x.Rank,
                    Genus = x.Genus,
                    Family = x.Family,
                    Order = x.Order,
                    Type = x.Type,
                };

                return animal;
            })
            .ToList();

        var countryDex = new CountryDex
        {
            AnimalCount = animals.Count,
            DateGenerated = DateTime.UtcNow,
            Animals = animals
        };

        var dexPath = Path.Combine(_outputPath, $"{countryData.Name.ToLower()}-dex.json.");

        var json = JsonSerializer.Serialize(countryDex, JsonConfigSettings.Options);

        await File.WriteAllTextAsync(dexPath, json);
        Console.WriteLine($"Created dex for country: `{countryData.Name}` in: {_outputPath}.");

        return new ActionResult(true);
    }
}