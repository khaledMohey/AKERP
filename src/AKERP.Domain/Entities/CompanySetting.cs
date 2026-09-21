namespace AKERP.Domain.Entities;

public class CompanySetting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
