# Builds the Inno Setup installer for SDI Converter 2: publish\SDI-Converter2-Setup-<version>.exe
# 1. publishes the GUI self-contained for win-x64 (this also builds the engine) into a staging folder outside the repository,
# 2. adds the VC++ runtime DLLs the engine needs (app-local, so no VC++ redistributable has to be installed),
# 3. compiles installer\sdi_converter2.iss with the version from gui\SdiConverter.csproj.
# Change the version in the csproj (and the ClickOnce profiles), not here.
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Split-Path $PSScriptRoot -Parent)).TrimEnd('\')
$csproj = Join-Path $root 'gui\SdiConverter.csproj'
$version = ([xml](Get-Content -LiteralPath $csproj -Raw -Encoding UTF8)).Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
if ($version -notmatch '^\d+\.\d+\.\d+\.\d+$') { throw "No version found in $csproj" }

$stagingRoot = [IO.Path]::GetFullPath((Join-Path $env:LOCALAPPDATA 'sdi_converter2')) + '\'
$payload = [IO.Path]::GetFullPath((Join-Path $stagingRoot 'setup-payload'))
if (!$payload.StartsWith($stagingRoot, [StringComparison]::OrdinalIgnoreCase)) { throw 'Staging path is outside SDI Converter 2' }
if (Test-Path -LiteralPath $payload) { Remove-Item -LiteralPath $payload -Recurse -Force }

& dotnet publish $csproj -c Release -r win-x64 --self-contained true -p:UseSharedCompilation=false -p:DebugType=none -o $payload
if ($LASTEXITCODE) { throw 'Publish failed' }
foreach ($file in 'SdiConverter2.exe', 'sdi_converter_engine.exe', 'libomt.dll', 'libvmx.dll') {
    if (!(Test-Path -LiteralPath (Join-Path $payload $file))) { throw "$file missing in the payload" }
}

$vs = Join-Path $env:ProgramFiles 'Microsoft Visual Studio'
$crt = Get-ChildItem -Path $vs -Directory -Filter 'Microsoft.VC*.CRT' -Recurse -Depth 7 -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match '\\VC\\Redist\\MSVC\\[^\\]+\\x64\\' } | Sort-Object FullName -Descending | Select-Object -First 1
if (!$crt) { throw 'VC++ runtime (VC\Redist\MSVC\...\x64\Microsoft.VC*.CRT) not found' }
foreach ($dll in 'msvcp140.dll', 'vcruntime140.dll', 'vcruntime140_1.dll') {
    Copy-Item -LiteralPath (Join-Path $crt.FullName $dll) -Destination $payload -Force
}

$iscc = @("$env:ProgramFiles\Inno Setup 7\ISCC.exe", "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe") | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
if (!$iscc) { throw 'Inno Setup compiler (ISCC.exe) not found' }
& $iscc "/DMyAppVersion=$version" "/DPayload=$payload" (Join-Path $PSScriptRoot 'sdi_converter2.iss')
if ($LASTEXITCODE) { throw 'Inno Setup failed' }
Write-Host "Ready: $root\publish\SDI-Converter2-Setup-$version.exe"
