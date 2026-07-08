using System.Text.Json;
using Database;
using Microsoft.EntityFrameworkCore;
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

    public async Task<ActionResult> CreateDex(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode))
            return new ActionResult(false, $"Expected input country code is empty: {countryCode}.");

        var countryDistributionIds = _dbContext.Distributions.Where(x =>
            x.AreaId.Equals(countryCode)).Select(x => x.ColId).ToList();

        if (countryDistributionIds.Count == 0)
            return new ActionResult(false, $"No matching countries found for code {countryCode}.");

        var allSpecies = _dbContext.Taxa.Where(x => x.Rank.Equals("species")).ToList();

        var countrySpecies = allSpecies.IntersectBy(countryDistributionIds, x => x.ColId).ToList();


        //var countrySpecies = allSpecies.Where(x => countryDistributionIds.Contains(x.ColId));
        //.Select(x => x.ColId).Intersect(countryDistributionIds);
        //.Where(x => countryDistributionIds..Equals(x.ColId));//.ToList();
        //.Contains(x.ColId)).ToList();

        var countryDex = countrySpecies.Select(x => new ColAnimalData
            {
                VernacularName = "",
                ScientificName = x.ScientificName,
                Rank = x.Rank,
                Genus = x.Genus,
                Family = x.Family,
                Order = x.Order,
                Type = x.Type,
                CountyCode = countryCode
            })
            .ToList();

        var dexPath = Path.Combine(_outputPath, $"{countryCode}-dex.json");

        var json = JsonSerializer.Serialize(countryDex, JsonConfigSettings.Options);

        await File.WriteAllTextAsync(json, dexPath);
        Console.WriteLine($"created dex for country code: {countryCode} in: {_outputPath}");

        return new ActionResult(true);
    }
}