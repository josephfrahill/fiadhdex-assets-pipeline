using FiadhDex.Database.DbModels;
using Microsoft.EntityFrameworkCore;

namespace FiadhDex.Database;

public class FiadhDexDbContext(DbContextOptions<FiadhDexDbContext> options) : DbContext(options)
{
    public DbSet<Taxon> Taxa => Set<Taxon>();
    public DbSet<VernacularName> VernacularNames => Set<VernacularName>();
    public DbSet<ColDistribution> ColDistributions => Set<ColDistribution>();
    public DbSet<GbifAnnualOccurrence> GbifAnnualOccurrences => Set<GbifAnnualOccurrence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Taxon>()
            .HasKey(x => x.ColId);

        modelBuilder.Entity<VernacularName>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<VernacularName>()
            .HasIndex(x => x.Language);

        modelBuilder.Entity<VernacularName>()
            .HasOne<Taxon>()
            .WithMany(t => t.VernacularNames)
            .HasForeignKey(x => x.ColId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ColDistribution>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ColDistribution>()
            .HasOne<Taxon>()
            .WithMany(t => t.ColDistributions)
            .HasForeignKey(x => x.ColId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GbifAnnualOccurrence>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<GbifAnnualOccurrence>()
            .HasOne<Taxon>()
            .WithMany(t => t.GbifAnnualOccurrences)
            .HasForeignKey(x => x.ColId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GbifAnnualOccurrence>().HasIndex(x => new
        {
            x.ColId,
            x.CountryCode,
            x.Year
        }).IsUnique();

        // Note: EF Core automatically creates an index on Foreign Key columns
    }
}