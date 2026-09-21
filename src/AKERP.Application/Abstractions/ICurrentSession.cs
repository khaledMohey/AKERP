using AKERP.Domain.Entities;
using AKERP.Domain.Enums;

namespace AKERP.Application.Abstractions;

public sealed record SessionInfo(
    Guid UserId,
    Guid CompanyId,
    string UserName,
    string DisplayName,
    string Role,
    string CompanyName,
    LicenseType LicenseType,
    LicenseStatus LicenseStatus);

public interface ICurrentSession
{
    SessionInfo? Current { get; }
    event Action? Changed;
    Task LoadAsync();
    Task SetAsync(SessionInfo session);
    Task ClearAsync();
    bool IsAuthenticated => Current is not null;
}
