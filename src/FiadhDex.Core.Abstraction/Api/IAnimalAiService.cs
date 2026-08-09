using FiadhDex.Models;
using FiadhDex.Models.Dto;

namespace FiadhDex.Core.Abstraction.Api;

public interface IOpenAiEnrichmentService
{
    Task<ActionResult> EnrichBaseDexWithOpenAiAsync(
        CancellationToken cancellationToken = default);
}