param(
    [string]$ServerIp = "127.0.0.1"
)

$ErrorActionPreference = "Stop"
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
if (-not (Test-Path (Join-Path $root "AKERP.sln"))) {
    $root = "H:\AK Tech\AKERP"
}

$out = Join-Path $root "deploy\lan\DesktopClient"
New-Item -ItemType Directory -Force -Path $out | Out-Null

Write-Host "Publishing desktop client to $out ..."
dotnet publish (Join-Path $root "src\AKERP.Desktop\AKERP.Desktop.csproj") -c Release -r win-x64 --self-contained false -o $out

$serverJson = @{ ServerUrl = "http://$ServerIp`:5088" } | ConvertTo-Json
Set-Content -Path (Join-Path $out "server.json") -Value $serverJson -Encoding UTF8

Write-Host ""
Write-Host "Done."
Write-Host "1) Copy folder: $out"
Write-Host "2) Edit server.json if needed -> http://$ServerIp`:5088"
Write-Host "3) On server PC run: dotnet run --project src/AKERP.Web --launch-profile lan"
