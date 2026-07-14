using Database.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class LifeDexDbContext(DbContextOptions<LifeDexDbContext> options) : DbContext(options)
{
    public DbSet<Taxon> Taxa => Set<Taxon>();
    public DbSet<VernacularName> VernacularNames => Set<VernacularName>();
    public DbSet<ColDistribution> ColDistributions => Set<ColDistribution>();

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
            .OnDelete(DeleteBehavior.Cascade); // Automatically clean up orphans

        modelBuilder.Entity<ColDistribution>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ColDistribution>()
            .HasOne<Taxon>()
            .WithMany(t => t.ColDistributions)
            .HasForeignKey(x => x.ColId)
            .OnDelete(DeleteBehavior.Cascade);

        // Note: EF Core automatically creates an index on Foreign Key columns
    }
}