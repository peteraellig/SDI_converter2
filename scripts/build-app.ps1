$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
& dotnet publish (Join-Path $root 'gui\SdiConverter.csproj') -c Release --self-contained false -p:UseSharedCompilation=false -o (Join-Path $root 'app')
if ($LASTEXITCODE) { throw 'Build failed' }
Write-Host "Ready: $root\app\SdiConverter2.exe"
