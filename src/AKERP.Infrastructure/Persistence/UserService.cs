using AKERP.Application.Abstractions;
using AKERP.Domain.Entities;
using AKERP.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace AKERP.Infrastructure.Persistence;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<UserListItem>> GetByCompanyAsync(Guid companyId, CancellationToken ct = default)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.UserName)
            .Select(x => new UserListItem(x.Id, x.UserName, x.DisplayName, x.Role, x.IsActive))
            .ToListAsync(ct);
    }

    public async Task<Guid> CreateAsync(Guid companyId, string userName, string displayName, string password, string role, CancellationToken ct = default)
    {
        var exists = await _db.Users.AnyAsync(x => x.UserName == userName.Trim(), ct);
        if (exists)
            throw new InvalidOperationException("اسم المستخدم مستخدم بالفعل على مستوى النظام");

        var user = new AppUser
        {
            CompanyId = companyId,
            UserName = userName.Trim(),
            DisplayName = displayName.Trim(),
            PasswordHash = PasswordHasher.Hash(password),
            Role = string.IsNullOrWhiteSpace(role) ? "User" : role.Trim(),
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task SetActiveAsync(Guid userId, bool isActive, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId, ct)
            ?? throw new InvalidOperationException("المستخدم غير موجود");
        user.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<(bool Ok, string? Error, SessionInfo? Session)> AuthenticateAsync(string userName, string password, CancellationToken ct = default)
    {
        var user = await _db.Users
            .Include(x => x.Company)
            .ThenInclude(c => c.License)
            .FirstOrDefaultAsync(x => x.UserName == userName.Trim(), ct);

        if (user is null || !PasswordHasher.Verify(password, user.PasswordHash))
            return (false, "اسم المستخدم أو كلمة المرور غير صحيحة", null);

        if (!user.IsActive)
            return (false, "المستخدم غير مفعّل", null);

        if (!user.Company.IsActive)
            return (false, "الشركة غير مفعّلة", null);

        var license = user.Company.License;
        if (license is null)
            return (false, "لا توجد رخصة للشركة", null);

        if (license.Status == Domain.Enums.LicenseStatus.Suspended)
            return (false, "الرخصة موقوفة", null);

        if (license.Status == Domain.Enums.LicenseStatus.Expired ||
            (license.ExpiresAtUtc is not null && license.ExpiresAtUtc < DateTime.UtcNow))
            return (false, "الرخصة منتهية", null);

        var session = new SessionInfo(
            user.Id,
            user.CompanyId,
            user.UserName,
            user.DisplayName,
            user.Role,
            user.Company.Name,
            license.Type,
            license.Status);

        return (true, null, session);
    }
}
