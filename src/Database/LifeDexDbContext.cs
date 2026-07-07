using Database.DbModels;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Database;

public class LifeDexDbContext : DbContext
{
    private static readonly string DbDirectory = Path.Combine(Utils.GetSolutionDirectory(), "db");

    public LifeDexDbContext()
    {
        Directory.CreateDirectory(DbDirectory);
    }

    public DbSet<Taxon> Taxa => Set<Taxon>();
    public DbSet<VernacularName> VernacularNames => Set<VernacularName>();
    public DbSet<Distribution> Distributions => Set<Distribution>();
    public string DbPath { get; } = Path.Combine(DbDirectory, "lifedex.db");

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Taxon>()
            .HasKey(x => x.ColId);

        modelBuilder.Entity<VernacularName>()
            .HasIndex(x => x.CatalogueOfLifeId);

        modelBuilder.Entity<Distribution>()
            .HasIndex(x => x.CatalogueOfLifeId);
    }
}