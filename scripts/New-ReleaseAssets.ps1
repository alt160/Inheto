param(
    [string] $Version = '1.0.1',
    [string] $PackageDirectory = (Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts/packages'),
    [string] $LibraryDirectory = (Join-Path (Split-Path $PSScriptRoot -Parent) 'bin/Release/net8.0'))

$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+$') { throw 'A stable major.minor.patch version is required.' }
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskPackages = [IO.Path]::GetFullPath($PackageDirectory)
[IO.Directory]::CreateDirectory($taskPackages) | Out-Null
$taskArchive = Join-Path $taskPackages "Inheto-$Version-net8.0.zip"
if (Test-Path -LiteralPath $taskArchive) { throw "Refusing to replace an existing release archive: $taskArchive" }
Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz') Create precompiled library archive and checksums; no publishing."
Add-Type -AssemblyName System.IO.Compression
$taskZip = [IO.Compression.ZipFile]::Open($taskArchive, [IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($taskName in @('InhetoSerializer.dll', 'InhetoSerializer.xml', 'InhetoSerializer.pdb')) {
        $taskPath = Join-Path $LibraryDirectory $taskName
        if (-not (Test-Path -LiteralPath $taskPath -PathType Leaf)) { throw "Missing compiled library file: $taskPath" }
        [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($taskZip, $taskPath, $taskName) | Out-Null
    }
    foreach ($taskName in @('README.md', 'LICENSE', 'CHANGELOG.md', 'docs/types-and-round-tripping.md',
        'docs/property-paths.md', 'docs/performance-and-memory.md', 'docs/comparisons.md',
        'docs/configuration-and-reconstruction.md', 'docs/manual-assembly-use.md')) {
        [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($taskZip, (Join-Path $taskRoot $taskName), $taskName) | Out-Null
    }
}
finally { $taskZip.Dispose() }
$taskAssets = @("Inheto.$Version.nupkg", "Inheto.$Version.snupkg", "Inheto-$Version-net8.0.zip")
$taskLines = foreach ($taskAsset in $taskAssets) {
    $taskFile = Join-Path $taskPackages $taskAsset
    if (-not (Test-Path -LiteralPath $taskFile -PathType Leaf)) { throw "Missing release asset: $taskFile" }
    (Get-FileHash -LiteralPath $taskFile -Algorithm SHA256).Hash.ToLowerInvariant() + '  ' + $taskAsset
}
[IO.File]::WriteAllLines((Join-Path $taskPackages 'SHA256SUMS.txt'), $taskLines, [Text.UTF8Encoding]::new($false))
Write-Host "Created $taskArchive and SHA256SUMS.txt. The archive contains the library, not its separately licensed NuGet dependencies."
