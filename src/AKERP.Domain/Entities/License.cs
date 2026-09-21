using AKERP.Domain.Enums;

namespace AKERP.Domain.Entities;

public class License
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public LicenseType Type { get; set; } = LicenseType.Subscription;
    public LicenseStatus Status { get; set; } = LicenseStatus.Active;
    public string LicenseKey { get; set; } = string.Empty;
    public DateTime? ExpiresAtUtc { get; set; }
    public int MaxUsers { get; set; } = 5;
    public int MaxDevices { get; set; } = 5;
    public string ModulesCsv { get; set; } = "dashboard,inventory,sales,purchase,partners,reps,accounting,reports";
    public DateTime ActivatedAtUtc { get; set; } = DateTime.UtcNow;
}
