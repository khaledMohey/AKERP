using AKERP.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AKERP.Infrastructure.Persistence;

public class FeatureService : IFeatureService
{
    private readonly AppDbContext _db;

    public FeatureService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> IsEnabledAsync(Guid companyId, string featureKey, CancellationToken cancellationToken = default)
    {
        return await _db.CompanyFeatures
            .AsNoTracking()
            .AnyAsync(x => x.CompanyId == companyId && x.FeatureKey == featureKey && x.IsEnabled, cancellationToken);
    }

    public async Task<string?> GetSettingAsync(Guid companyId, string key, CancellationToken cancellationToken = default)
    {
        return await _db.CompanySettings
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.Key == key)
            .Select(x => x.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
