using System.Text.Json;
using Lifedex.Concrete.Json;
using Lifedex.Constants.Countries;
using Lifedex.Constants.Exclusions;
using Lifedex.Database;
using Lifedex.Models;
using Lifedex.Models.AnimalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Lifedex.Concrete.Dex;

public class DexCreator
{
    private readonly LifeDexDbContext _dbContext;
    private readonly PipelineConfig _config;
    private readonly DexFetcher _dexFetcher;
    private readonly string _dexesOutputPath;
    private CountryDex _globalDex = null!;

    private DexCreator(LifeDexDbContext context, IOptions<PipelineConfig> options, DexFetcher dexFetcher)
    {
        _dbContext = context;
        _config = options.Value;
        _dexFetcher = dexFetcher;

        _dexesOutputPath = Path.Combine(_config.SolutionRoot, _config.Folders.Output, _config.Folders.Dexes);
        Directory.CreateDirectory(_dexesOutputPath);
    }

    public static async Task<DexCreator> InitialiseAsync(LifeDexDbContext context, IOptions<PipelineConfig> options,
        DexFetcher dexFetcher)
    {
        var creator = new DexCreator(context, options, dexFetcher);
        await creator.LoadGlobalDexAsync();
        return creator;
    }

    private async Task LoadGlobalDexAsync()
    {
        var fetchResult = await
            _dexFetcher.FetchDexAsync(_config.DexConfig
                .GlobalDexName);

        if (!fetchResult.Successful || fetchResult.CountryDex is null)
            throw new JsonException(fetchResult.ErrorMessage);

        _globalDex = fetchResult.CountryDex;
    }

    public async Task<ActionResult> CreateCountryDexBase(string givenCountry)
    {
        var countryValidated = givenCountry.ToLower().Trim();

        if (string.IsNullOrEmpty(countryValidated))
        {
            return new ActionResult(false)
            {
                ErrorMessage = $"Expected input country is empty: {givenCountry}."
            };
        }

        if (CountryLookup.TryParse(givenCountry, out var countryData))
        {
            Console.WriteLine($"Found: {countryData.Name} [{countryData.Code}].");
        }
        else
        {
            return new ActionResult(false)
            {
                ErrorMessage = $"No country found for: `{givenCountry}`."
            };
        }

        /*
        var countryDistributionIds = _dbContext.ColDistributions.Where(x =>
            x.Area.Contains(countryData.Name) == true).Select(x => x.ColId).ToList();
        */

        var countryOccurrencesIds = _dbContext.GbifAnnualOccurrences.Where(x =>
            x.CountryCode.Equals(countryData.Code)).Select(x => x.ColId);

        if (!countryOccurrencesIds.Any())
        {
            return new ActionResult(false)
            {
                ErrorMessage = $"No matching countries found for {countryData.Name}."
            };
        }

        // will need to be smarter for v2 as birds are all species
        var rank = _config.DexConfig.IgnoreSubspecies ? "species" : "subspecies";

        var allSpecies = _dbContext.Taxa.Where(x => x.Rank.Equals(rank)).Include(taxon => taxon.VernacularNames)
            .ToList();

        var countrySpecies = allSpecies.IntersectBy(countryOccurrencesIds, x => x.ColId).ToList();

        var globalScientificNames = _globalDex.Animals.Select(x => x.ScientificName).ToArray();

        var countrySpeciesWithoutGlobals = countrySpecies
            .Where(x => !globalScientificNames.Contains(x.ScientificName, StringComparer.OrdinalIgnoreCase)).ToArray();

        var countryAnimalsFilteredOrders = countrySpeciesWithoutGlobals
            .Where(x => !DexExclusions.ExcludedOrders.Contains(x.Order, StringComparer.OrdinalIgnoreCase));

        var countryAnimalsFilteredFamilies = countryAnimalsFilteredOrders
            .Where(x => !DexExclusions.ExcludedFamilies.Contains(x.Order, StringComparer.OrdinalIgnoreCase));

        var animals = countryAnimalsFilteredFamilies
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
                    Rank = ToTitleCase(x.Rank),
                    Genus = x.Genus,
                    Family = x.Family,
                    Order = x.Order,
                    Type = x.Type,
                };

                return animal;
            })
            .ToList();

        var countryDex = new CountryDexBase
        {
            TotalCount = animals.Count,
            AmphibiaCount = animals.Count(x => x.Type.Equals("Amphibia")),
            AvesCount = animals.Count(x => x.Type.Equals("Aves")),
            MammaliaCount = animals.Count(x => x.Type.Equals("Mammalia")),
            ReptiliaCount = animals.Count(x => x.Type.Equals("Reptilia")),
            DateGenerated = DateTime.UtcNow,
            Animals = animals
        };

        var differenceCount = countrySpecies.Count - countrySpeciesWithoutGlobals.Length;
        Console.WriteLine($"Stripped {differenceCount} global entries from new dex of total {animals.Count} count.");

        var dexPath = Path.Combine(_dexesOutputPath, $"{countryData.Name.ToLower()}-dex.json.");

        var json = JsonSerializer.Serialize(countryDex, JsonConfigSettings.Options);

        await File.WriteAllTextAsync(dexPath, json);
        Console.WriteLine($"Created dex for country: `{countryData.Name}` in: {_dexesOutputPath}.");

        return new ActionResult(true);
    }

    public static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input[1..];
    }
}