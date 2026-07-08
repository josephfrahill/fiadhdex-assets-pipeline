using Database;
using Microsoft.Extensions.Options;
using Models;

namespace Services.DexCreation;

public class DexCreator
{
    private readonly LifeDexDbContext _context;
    private readonly PipelineConfig _config;

    public DexCreator(LifeDexDbContext context, IOptions<PipelineConfig> options)
    {
        _context = context;
        _config = options.Value;
    }
}