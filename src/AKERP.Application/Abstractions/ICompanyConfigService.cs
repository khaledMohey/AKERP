namespace AKERP.Application.Abstractions;

public sealed record FeatureItem(string Key, string TitleAr, bool IsEnabled);
public sealed record SettingItem(string Key, string Value);

public interface ICompanyConfigService
{
    Task<IReadOnlyList<FeatureItem>> GetFeaturesAsync(Guid companyId, CancellationToken ct = default);
    Task SetFeatureAsync(Guid companyId, string featureKey, bool enabled, CancellationToken ct = default);
    Task<IReadOnlyList<SettingItem>> GetSettingsAsync(Guid companyId, CancellationToken ct = default);
    Task UpsertSettingAsync(Guid companyId, string key, string value, CancellationToken ct = default);
}
