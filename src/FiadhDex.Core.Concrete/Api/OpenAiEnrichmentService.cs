using FiadhDex.Core.Abstraction.Api;
using FiadhDex.Core.Concrete.Json;
using FiadhDex.Database;
using FiadhDex.Database.DbModels;
using FiadhDex.Models;
using FiadhDex.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI;

namespace FiadhDex.Core.Concrete.Api;

public sealed class OpenAiEnrichmentService : IOpenAiEnrichmentService
{
    private readonly OpenAIClient _client;
    private readonly FiadhDexDbContext _dbContext;
    private readonly PipelineConfig _config;
    private const string _model = "gpt-5.4-mini";
    private const string _promptVersion = "1.0";

    public OpenAiEnrichmentService(IOptions<OpenAiOptions> options, FiadhDexDbContext dbContext, IOptions<PipelineConfig> config)
    {
        _client = new OpenAIClient(options.Value.ApiKey);
        _dbContext = dbContext;
        _config = config.Value;
    }

    public async Task<ActionResult> EnrichBaseDexWithOpenAiAsync(
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_config.OpenAiConfig.WorkingDexName))
        {
            return new ActionResult(false, "WorkingDexName is not configured.");
        }

        var workingDexPath = Path.Combine(_config.SolutionRoot, _config.Folders.Output,
            _config.Folders.Dexes, _config.OpenAiConfig.WorkingDexName);

        var workingDex = await JsonDexLoader.LoadAsync<CountryDexBase>(workingDexPath);

        //var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        
        foreach (var animal in workingDex.Animals)
        {
            if (await _dbContext.AiEnrichments.SingleAsync(a => a.ColId == animal.ColId, cancellationToken: cancellationToken) != null)
            {
                continue;
            }

            // TODO:
            // Build prompt
            // Call GPT  //new AnimalAiDto
            // Deserialize JSON
            // Validate
            // Return AnimalAiData

            //save to db per request so we don't lose any data, even if less optimal
            // if animal exists, just append geo regions

            var enrichmentEntry = new AiEnrichment
            {
                ColId = animal.ColId,
                Model = _model,
                PromptVersion = _promptVersion,
                Data = "{}" // Placeholder for actual enriched data
            };

            _dbContext.AiEnrichments.Add(enrichmentEntry);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new ActionResult(true);
    }
}