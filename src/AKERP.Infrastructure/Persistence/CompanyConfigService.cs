using AKERP.Application.Abstractions;
using AKERP.Domain;
using AKERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AKERP.Infrastructure.Persistence;

public class CompanyConfigService : ICompanyConfigService
{
    private readonly AppDbContext _db;

    public CompanyConfigService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<FeatureItem>> GetFeaturesAsync(Guid companyId, CancellationToken ct = default)
    {
        var enabled = await _db.CompanyFeatures
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .ToDictionaryAsync(x => x.FeatureKey, x => x.IsEnabled, ct);

        return FeatureKeys.Catalog
            .Select(f => new FeatureItem(f.Key, f.TitleAr, enabled.TryGetValue(f.Key, out var on) && on))
            .ToList();
    }

    public async Task SetFeatureAsync(Guid companyId, string featureKey, bool enabled, CancellationToken ct = default)
    {
        var row = await _db.CompanyFeatures
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.FeatureKey == featureKey, ct);

        if (row is null)
        {
            _db.CompanyFeatures.Add(new CompanyFeature
            {
                CompanyId = companyId,
                FeatureKey = featureKey,
                IsEnabled = enabled
            });
        }
        else
        {
            row.IsEnabled = enabled;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<SettingItem>> GetSettingsAsync(Guid companyId, CancellationToken ct = default)
    {
        return await _db.CompanySettings
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Key)
            .Select(x => new SettingItem(x.Key, x.Value))
            .ToListAsync(ct);
    }

    public async Task UpsertSettingAsync(Guid companyId, string key, string value, CancellationToken ct = default)
    {
        var row = await _db.CompanySettings
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Key == key, ct);

        if (row is null)
        {
            _db.CompanySettings.Add(new CompanySetting
            {
                CompanyId = companyId,
                Key = key.Trim(),
                Value = value
            });
        }
        else
        {
            row.Value = value;
        }

        await _db.SaveChangesAsync(ct);
    }
}
