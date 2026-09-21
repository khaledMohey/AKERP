using AKERP.Application.Abstractions;
using AKERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AKERP.Infrastructure.Persistence;

public class LicenseService : ILicenseService
{
    private readonly AppDbContext _db;

    public LicenseService(AppDbContext db) => _db = db;

    public async Task<LicenseDetails?> GetByCompanyAsync(Guid companyId, CancellationToken ct = default)
    {
        return await _db.Licenses
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .Select(x => new LicenseDetails(
                x.Id,
                x.CompanyId,
                x.Type,
                x.Status,
                x.LicenseKey,
                x.ExpiresAtUtc,
                x.MaxUsers,
                x.MaxDevices,
                x.ModulesCsv))
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpdateAsync(Guid companyId, LicenseType type, LicenseStatus status, DateTime? expiresAtUtc, int maxUsers, int maxDevices, string modulesCsv, CancellationToken ct = default)
    {
        var license = await _db.Licenses.FirstOrDefaultAsync(x => x.CompanyId == companyId, ct)
            ?? throw new InvalidOperationException("الرخصة غير موجودة");

        license.Type = type;
        license.Status = status;

        if (type == LicenseType.Perpetual)
        {
            license.ExpiresAtUtc = null;
        }
        else
        {
            license.ExpiresAtUtc = expiresAtUtc ?? DateTime.UtcNow.AddYears(1);
        }

        license.MaxUsers = Math.Max(1, maxUsers);
        license.MaxDevices = Math.Max(1, maxDevices);
        license.ModulesCsv = string.IsNullOrWhiteSpace(modulesCsv)
            ? "dashboard,inventory,sales,purchase,partners,reps,accounting,reports"
            : modulesCsv.Trim();

        await _db.SaveChangesAsync(ct);
    }
}
