$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$output = Join-Path $env:LOCALAPPDATA 'sdi_converter2\channel-tests'
& dotnet build (Join-Path $root 'tests\FakeEngine\FakeEngine.csproj') -c Release -o $output -p:UseSharedCompilation=false
if ($LASTEXITCODE) { throw 'Fake engine build failed' }
& dotnet build (Join-Path $root 'tests\ChannelHarness\ChannelHarness.csproj') -c Release -o $output -p:UseSharedCompilation=false
if ($LASTEXITCODE) { throw 'Harness build failed' }
& (Join-Path $output 'ChannelHarness.exe') --test
if ($LASTEXITCODE) { throw 'Channel integration checks failed' }
