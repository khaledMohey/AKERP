using System.Text.Json;
using AKERP.Application.Abstractions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace AKERP.Web.Services;

public class BrowserCurrentSession : ICurrentSession
{
    private const string StorageKey = "akerp.session";
    private readonly ProtectedSessionStorage _storage;
    private SessionInfo? _current;
    private bool _loaded;

    public BrowserCurrentSession(ProtectedSessionStorage storage)
    {
        _storage = storage;
    }

    public SessionInfo? Current => _current;
    public event Action? Changed;

    public async Task LoadAsync()
    {
        if (_loaded) return;
        _loaded = true;

        try
        {
            var result = await _storage.GetAsync<string>(StorageKey);
            if (result.Success && !string.IsNullOrWhiteSpace(result.Value))
            {
                _current = JsonSerializer.Deserialize<SessionInfo>(result.Value);
                Changed?.Invoke();
            }
        }
        catch
        {
            _current = null;
        }
    }

    public async Task SetAsync(SessionInfo session)
    {
        _current = session;
        _loaded = true;
        await _storage.SetAsync(StorageKey, JsonSerializer.Serialize(session));
        Changed?.Invoke();
    }

    public async Task ClearAsync()
    {
        _current = null;
        _loaded = true;
        await _storage.DeleteAsync(StorageKey);
        Changed?.Invoke();
    }
}
