namespace AKERP.Application.Abstractions;

public interface IFeatureService
{
    Task<bool> IsEnabledAsync(Guid companyId, string featureKey, CancellationToken cancellationToken = default);
    Task<string?> GetSettingAsync(Guid companyId, string key, CancellationToken cancellationToken = default);
}
