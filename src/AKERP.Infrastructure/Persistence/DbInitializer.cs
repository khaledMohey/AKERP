using AKERP.Domain;
using AKERP.Domain.Entities;
using AKERP.Domain.Enums;
using AKERP.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKERP.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        if (await db.Companies.AnyAsync())
            return;

        var company = new Company
        {
            Name = "شركة تجريبية",
            Code = "DEMO",
            IsActive = true
        };

        company.License = new License
        {
            Type = LicenseType.Subscription,
            Status = LicenseStatus.Active,
            LicenseKey = "AK-DEMO-SUB-001",
            MaxUsers = 10,
            MaxDevices = 10,
            ExpiresAtUtc = DateTime.UtcNow.AddYears(1),
            ModulesCsv = "dashboard,inventory,sales,purchase,partners,reps,accounting,reports"
        };

        company.Users.Add(new AppUser
        {
            UserName = "admin",
            DisplayName = "مدير النظام",
            PasswordHash = PasswordHasher.Hash("admin123"),
            Role = "Admin",
            IsActive = true
        });

        company.Features.Add(new CompanyFeature { FeatureKey = FeatureKeys.MultiWarehouse, IsEnabled = true });
        company.Features.Add(new CompanyFeature { FeatureKey = FeatureKeys.VehicleNumber, IsEnabled = false });

        company.Settings.Add(new CompanySetting { Key = SettingKeys.Currency, Value = "EGP" });
        company.Settings.Add(new CompanySetting { Key = SettingKeys.TaxPercent, Value = "14" });
        company.Settings.Add(new CompanySetting { Key = SettingKeys.CommissionMethod, Value = "flat" });

        db.Companies.Add(company);
        await db.SaveChangesAsync();
    }
}
