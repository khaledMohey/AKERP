using AKERP.Application.Abstractions;
using AKERP.Infrastructure;
using AKERP.Infrastructure.Persistence;
using AKERP.Web.Components;
using AKERP.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "akerp.dev.db");
builder.Services.AddInfrastructure($"Data Source={dbPath}", useSqlite: true);
builder.Services.AddScoped<ICurrentSession, BrowserCurrentSession>();

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

var isLan = string.Equals(Environment.GetEnvironmentVariable("AKERP_LAN"), "true", StringComparison.OrdinalIgnoreCase);
if (!isLan)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
