using AKERP.Domain.Enums;

namespace AKERP.Application.Abstractions;

public sealed record LicenseDetails(
    Guid Id,
    Guid CompanyId,
    LicenseType Type,
    LicenseStatus Status,
    string LicenseKey,
    DateTime? ExpiresAtUtc,
    int MaxUsers,
    int MaxDevices,
    string ModulesCsv);

public interface ILicenseService
{
    Task<LicenseDetails?> GetByCompanyAsync(Guid companyId, CancellationToken ct = default);
    Task UpdateAsync(Guid companyId, LicenseType type, LicenseStatus status, DateTime? expiresAtUtc, int maxUsers, int maxDevices, string modulesCsv, CancellationToken ct = default);
}
