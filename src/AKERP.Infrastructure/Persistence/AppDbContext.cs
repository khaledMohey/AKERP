using AKERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AKERP.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<CompanyFeature> CompanyFeatures => Set<CompanyFeature>();
    public DbSet<CompanySetting> CompanySettings => Set<CompanySetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Code).HasMaxLength(50);
            e.HasOne(x => x.License).WithOne(x => x.Company).HasForeignKey<License>(x => x.CompanyId);
        });

        modelBuilder.Entity<License>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.LicenseKey).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.LicenseKey).IsUnique();
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserName).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.CompanyId, x.UserName }).IsUnique();
        });

        modelBuilder.Entity<CompanyFeature>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FeatureKey).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.CompanyId, x.FeatureKey }).IsUnique();
        });

        modelBuilder.Entity<CompanySetting>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Key).HasMaxLength(100).IsRequired();
            e.Property(x => x.Value).HasMaxLength(4000).IsRequired();
            e.HasIndex(x => new { x.CompanyId, x.Key }).IsUnique();
        });
    }
}
