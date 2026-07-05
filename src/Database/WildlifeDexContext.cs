using Database.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class WildlifeDexContext : DbContext
{
    public DbSet<Species> Species => Set<Species>();
    public DbSet<VernacularName> VernacularNames => Set<VernacularName>();
    public DbSet<Distribution> Distributions => Set<Distribution>();
    public string DbPath { get; }

    public WildlifeDexContext()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WildlifeDex");

        Directory.CreateDirectory(folder);

        DbPath = Path.Combine(folder, "lifedex.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Species>()
            .HasKey(x => x.CatalogueOfLifeId);

        modelBuilder.Entity<VernacularName>()
            .HasIndex(x => x.CatalogueOfLifeId);

        modelBuilder.Entity<Distribution>()
            .HasIndex(x => x.CatalogueOfLifeId);
    }
}