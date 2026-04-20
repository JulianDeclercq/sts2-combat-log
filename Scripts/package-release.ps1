<#
.SYNOPSIS
    Builds AdventureLog in Release config and produces dist/AdventureLog-vX.Y.Z.zip
    with the Nexus-ready layout (mods/AdventureLog/*.dll,*.json inside the zip).

.PARAMETER Publish
    If set, also creates a GitHub Release with the zip attached (requires gh CLI auth).

.EXAMPLE
    pwsh Scripts/package-release.ps1
    pwsh Scripts/package-release.ps1 -Publish
#>

param(
    [switch]$Publish
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')

$manifest = Get-Content (Join-Path $repoRoot 'AdventureLog.json') -Raw | ConvertFrom-Json
$version = $manifest.version
if (-not $version) { throw "version missing from AdventureLog.json" }

Write-Host "Packaging AdventureLog $version" -ForegroundColor Cyan

Push-Location $repoRoot
try {
    & dotnet build -c Release --nologo --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }
} finally {
    Pop-Location
}

$dll  = Join-Path $repoRoot '.godot/mono/temp/bin/Release/AdventureLog.dll'
$json = Join-Path $repoRoot 'AdventureLog.json'
if (-not (Test-Path $dll))  { throw "DLL not found at $dll" }
if (-not (Test-Path $json)) { throw "Manifest not found at $json" }

$dist  = Join-Path $repoRoot 'dist'
$stage = Join-Path $dist 'stage/AdventureLog'
Remove-Item (Join-Path $dist 'stage') -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $stage -Force | Out-Null

Copy-Item $dll  (Join-Path $stage 'AdventureLog.dll')
Copy-Item $json (Join-Path $stage 'AdventureLog.json')

$zip = Join-Path $dist "AdventureLog-$version.zip"
Remove-Item $zip -Force -ErrorAction SilentlyContinue
# Build entries manually so paths use forward slashes on every PS / .NET version.
# (ZipFile.CreateFromDirectory on Windows PowerShell 5.1 / .NET Framework emits backslashes, which violates the ZIP spec.)
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$stageRoot = Join-Path $dist 'stage'
$zipStream = [System.IO.File]::Open($zip, [System.IO.FileMode]::Create)
try {
    $archive = New-Object System.IO.Compression.ZipArchive($zipStream, [System.IO.Compression.ZipArchiveMode]::Create)
    try {
        Get-ChildItem -Path $stageRoot -Recurse -File | ForEach-Object {
            $rel = $_.FullName.Substring($stageRoot.Length + 1).Replace('\','/')
            $entry = $archive.CreateEntry($rel, [System.IO.Compression.CompressionLevel]::Optimal)
            $entryStream = $entry.Open()
            try {
                $fs = [System.IO.File]::OpenRead($_.FullName)
                try { $fs.CopyTo($entryStream) } finally { $fs.Dispose() }
            } finally {
                $entryStream.Dispose()
            }
        }
    } finally {
        $archive.Dispose()
    }
} finally {
    $zipStream.Dispose()
}

Remove-Item (Join-Path $dist 'stage') -Recurse -Force

Write-Host "Zip -> $zip" -ForegroundColor Green

if ($Publish) {
    Write-Host "Creating GitHub Release $version"
    & gh release create $version $zip --title "AdventureLog $version" --generate-notes
    if ($LASTEXITCODE -ne 0) { throw "gh release create failed" }
    Write-Host "Release published." -ForegroundColor Green
}
