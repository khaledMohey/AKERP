namespace AKERP.Domain.Entities;

public class CompanyFeature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string FeatureKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
