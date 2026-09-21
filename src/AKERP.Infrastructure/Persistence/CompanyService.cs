using AKERP.Application.Abstractions;
using AKERP.Domain.Entities;
using AKERP.Domain.Enums;
using AKERP.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace AKERP.Infrastructure.Persistence;

public class CompanyService : ICompanyService
{
    private readonly AppDbContext _db;

    public CompanyService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<CompanyListItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Companies
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CompanyListItem(
                x.Id,
                x.Name,
                x.Code,
                x.IsActive,
                x.License != null ? x.License.Type : null,
                x.License != null ? x.License.Status : null,
                x.License != null ? x.License.LicenseKey : null,
                x.License != null ? x.License.ExpiresAtUtc : null,
                x.Users.OrderBy(u => u.UserName).Select(u => u.UserName).FirstOrDefault()))
            .ToListAsync(ct);
    }

    public async Task<CompanyDetails?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Companies
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CompanyDetails(x.Id, x.Name, x.Code, x.IsActive))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName))
            throw new InvalidOperationException("اسم الشركة مطلوب");

        if (string.IsNullOrWhiteSpace(request.AdminUserName) || string.IsNullOrWhiteSpace(request.AdminPassword))
            throw new InvalidOperationException("حساب الدخول للعميل مطلوب (مستخدم وكلمة مرور)");

        var userName = request.AdminUserName.Trim();
        if (await _db.Users.AnyAsync(x => x.UserName == userName, ct))
            throw new InvalidOperationException("اسم المستخدم مستخدم بالفعل — اختر اسمًا فريدًا");

        DateTime? expires = null;
        if (request.LicenseType == LicenseType.Subscription)
        {
            var months = request.MonthsOrZero <= 0 ? 12 : request.MonthsOrZero;
            expires = DateTime.UtcNow.AddMonths(months);
        }

        var company = new Company
        {
            Name = request.CompanyName.Trim(),
            Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim(),
            IsActive = true
        };

        var licenseKey = $"AK-{request.LicenseType.ToString().ToUpperInvariant()[..3]}-{Guid.NewGuid():N}"[..24].ToUpperInvariant();

        company.License = new License
        {
            Type = request.LicenseType,
            Status = LicenseStatus.Active,
            LicenseKey = licenseKey,
            MaxUsers = Math.Max(1, request.MaxUsers),
            MaxDevices = Math.Max(1, request.MaxUsers),
            ExpiresAtUtc = expires,
            ModulesCsv = "dashboard,inventory,sales,purchase,partners,reps,accounting,reports"
        };

        company.Users.Add(new AppUser
        {
            UserName = userName,
            DisplayName = string.IsNullOrWhiteSpace(request.AdminDisplayName) ? userName : request.AdminDisplayName.Trim(),
            PasswordHash = PasswordHasher.Hash(request.AdminPassword),
            Role = "Admin",
            IsActive = true
        });

        company.Settings.Add(new CompanySetting { Key = "currency", Value = "EGP" });
        company.Settings.Add(new CompanySetting { Key = "tax_percent", Value = "14" });
        company.Settings.Add(new CompanySetting { Key = "commission_method", Value = "flat" });

        _db.Companies.Add(company);
        await _db.SaveChangesAsync(ct);

        return new CreateCustomerResult(
            company.Id,
            company.Name,
            licenseKey,
            request.LicenseType,
            expires,
            userName);
    }

    public async Task UpdateAsync(Guid id, string name, string? code, bool isActive, CancellationToken ct = default)
    {
        var company = await _db.Companies.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new InvalidOperationException("الشركة غير موجودة");

        company.Name = name.Trim();
        company.Code = string.IsNullOrWhiteSpace(code) ? null : code.Trim();
        company.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
    }
}
