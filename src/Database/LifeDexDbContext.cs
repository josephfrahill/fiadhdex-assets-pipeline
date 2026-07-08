using Database.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class LifeDexDbContext(DbContextOptions<LifeDexDbContext> options) : DbContext(options)
{
    public DbSet<Taxon> Taxa => Set<Taxon>();
    public DbSet<VernacularName> VernacularNames => Set<VernacularName>();
    public DbSet<Distribution> Distributions => Set<Distribution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Taxon>()
            .HasKey(x => x.ColId);

        modelBuilder.Entity<VernacularName>()
            .HasIndex(x => x.ColId);

        modelBuilder.Entity<VernacularName>()
            .HasIndex(x => x.Language);

        modelBuilder.Entity<Distribution>()
            .HasIndex(x => x.ColId);
    }
}