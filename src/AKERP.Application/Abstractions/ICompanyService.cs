using AKERP.Domain.Enums;

namespace AKERP.Application.Abstractions;

public sealed record CompanyListItem(
    Guid Id,
    string Name,
    string? Code,
    bool IsActive,
    LicenseType? LicenseType,
    LicenseStatus? LicenseStatus,
    string? LicenseKey,
    DateTime? ExpiresAtUtc,
    string? AdminUserName);

public sealed record CompanyDetails(Guid Id, string Name, string? Code, bool IsActive);

public sealed record CreateCustomerRequest(
    string CompanyName,
    string? Code,
    LicenseType LicenseType,
    int MaxUsers,
    int MonthsOrZero,
    string AdminUserName,
    string AdminDisplayName,
    string AdminPassword);

public sealed record CreateCustomerResult(
    Guid CompanyId,
    string CompanyName,
    string LicenseKey,
    LicenseType LicenseType,
    DateTime? ExpiresAtUtc,
    string AdminUserName);

public interface ICompanyService
{
    Task<IReadOnlyList<CompanyListItem>> GetAllAsync(CancellationToken ct = default);
    Task<CompanyDetails?> GetAsync(Guid id, CancellationToken ct = default);
    Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, string name, string? code, bool isActive, CancellationToken ct = default);
}
