using AKERP.Domain.Enums;

namespace AKERP.Domain.Entities;

public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public License? License { get; set; }
    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<CompanyFeature> Features { get; set; } = new List<CompanyFeature>();
    public ICollection<CompanySetting> Settings { get; set; } = new List<CompanySetting>();
}
