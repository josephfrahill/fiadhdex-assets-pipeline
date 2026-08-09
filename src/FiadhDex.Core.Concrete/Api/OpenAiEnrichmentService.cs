using FiadhDex.Core.Abstraction.Api;
using FiadhDex.Core.Concrete.Json;
using FiadhDex.Database;
using FiadhDex.Models;
using FiadhDex.Models.Dto;
using Microsoft.Extensions.Options;
using OpenAI;

namespace FiadhDex.Core.Concrete.Api;

public sealed class OpenAiEnrichmentService : IOpenAiEnrichmentService
{
    private readonly OpenAIClient _client;
    private readonly FiadhDexDbContext _dbContext;
    private readonly PipelineConfig _config;

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

        foreach (var animal in workingDex.Animals)
        { 
        }
        // TODO:
        // Build prompt
        // Call GPT
        // Deserialize JSON
        // Validate
        // Return AnimalAiData
        //new AnimalAiDto

        //var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        return new ActionResult(true);
    }
}