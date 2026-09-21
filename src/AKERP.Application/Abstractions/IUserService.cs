namespace AKERP.Application.Abstractions;

public sealed record UserListItem(Guid Id, string UserName, string DisplayName, string Role, bool IsActive);

public interface IUserService
{
    Task<IReadOnlyList<UserListItem>> GetByCompanyAsync(Guid companyId, CancellationToken ct = default);
    Task<Guid> CreateAsync(Guid companyId, string userName, string displayName, string password, string role, CancellationToken ct = default);
    Task SetActiveAsync(Guid userId, bool isActive, CancellationToken ct = default);
    Task<(bool Ok, string? Error, SessionInfo? Session)> AuthenticateAsync(string userName, string password, CancellationToken ct = default);
}
