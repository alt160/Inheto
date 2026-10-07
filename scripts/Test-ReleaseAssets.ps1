param(
    [string] $Version = '1.0.0',
    [string] $PackageDirectory = (Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts/packages'))

$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+$') { throw 'A stable major.minor.patch version is required.' }
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskPackages = [IO.Path]::GetFullPath($PackageDirectory)
Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz') Validate the release package, symbols, library archive, and checksums."
Add-Type -AssemblyName System.IO.Compression
$taskPackage = [IO.Compression.ZipFile]::OpenRead((Join-Path $taskPackages "Inheto.$Version.nupkg"))
try {
    $taskExpected = @('Inheto.nuspec', 'lib/net8.0/InhetoSerializer.dll', 'lib/net8.0/InhetoSerializer.xml',
        'README.md', 'LICENSE', 'CHANGELOG.md', 'docs/types-and-round-tripping.md', 'docs/property-paths.md',
        'docs/performance-and-memory.md', 'docs/comparisons.md', 'docs/configuration-and-reconstruction.md')
    foreach ($taskEntry in $taskPackage.Entries) {
        $taskName = $taskEntry.FullName
        if ($taskName -notin $taskExpected -and $taskName -notmatch '^(_rels/|package/services/metadata/|\[Content_Types\]\.xml$|\.signature\.p7s$)') {
            throw "Unexpected runtime package entry: $taskName"
        }
    }
    foreach ($taskName in $taskExpected) {
        $taskEntry = $taskPackage.GetEntry($taskName)
        if ($null -eq $taskEntry) { throw "Missing runtime package entry: $taskName" }
        if ($taskName -match '\.md$|^LICENSE$') {
            $taskInput = $taskEntry.Open()
            $taskMemory = [IO.MemoryStream]::new()
            try {
                $taskInput.CopyTo($taskMemory)
                $taskBytes = [IO.File]::ReadAllBytes((Join-Path $taskRoot $taskName))
                if ([Convert]::ToBase64String($taskBytes) -cne [Convert]::ToBase64String($taskMemory.ToArray())) {
                    throw "Packaged documentation differs from source: $taskName"
                }
            }
            finally { $taskInput.Dispose(); $taskMemory.Dispose() }
        }
    }
    $taskReader = [IO.StreamReader]::new($taskPackage.GetEntry('Inheto.nuspec').Open())
    try { [xml] $taskSpec = $taskReader.ReadToEnd() } finally { $taskReader.Dispose() }
    $taskMetadata = $taskSpec.package.metadata
    if ($taskMetadata.id -cne 'Inheto' -or $taskMetadata.version -cne $Version -or $taskMetadata.authors -cne 'alt160' -or
        $taskMetadata.license.'#text' -cne 'Apache-2.0' -or $taskMetadata.license.type -cne 'expression' -or
        $taskMetadata.readme -cne 'README.md' -or $taskMetadata.repository.url -cne 'https://github.com/alt160/Inheto' -or
        $taskMetadata.repository.commit -notmatch '^[a-f0-9]{40}$') { throw 'Incorrect package identity, license, README, or source-revision metadata.' }
    $taskDependencies = @($taskMetadata.dependencies.group.dependency)
    if ($taskDependencies.Count -ne 2 -or (@($taskDependencies.id | Sort-Object) -join ',') -cne 'BufferStream,fasterflect') {
        throw 'Unexpected runtime dependencies.'
    }
    if (($taskDependencies | Where-Object id -eq 'BufferStream').version -cne '1.0.2' -or
        ($taskDependencies | Where-Object id -eq 'fasterflect').version -cne '3.0.0') { throw 'Unexpected runtime dependency versions.' }
    if ($taskSpec.OuterXml -match 'ReleaseBoundaryCheck|@[^\s<]+|[A-Z]:\\') { throw 'Private/review identity leaked into package metadata.' }
}
finally { $taskPackage.Dispose() }
$taskSymbols = [IO.Compression.ZipFile]::OpenRead((Join-Path $taskPackages "Inheto.$Version.snupkg"))
try {
    if ($null -eq $taskSymbols.GetEntry('lib/net8.0/InhetoSerializer.pdb')) { throw 'Missing portable symbols.' }
    if (@($taskSymbols.Entries | Where-Object FullName -match '\.(cs|dll)$').Count -ne 0) { throw 'Source or runtime DLL leaked into symbols package.' }
}
finally { $taskSymbols.Dispose() }
$taskBinary = [IO.Compression.ZipFile]::OpenRead((Join-Path $taskPackages "Inheto-$Version-net8.0.zip"))
try {
    $taskExpectedBinary = @('InhetoSerializer.dll', 'InhetoSerializer.xml', 'InhetoSerializer.pdb', 'README.md', 'LICENSE',
        'CHANGELOG.md', 'docs/types-and-round-tripping.md', 'docs/property-paths.md', 'docs/performance-and-memory.md',
        'docs/comparisons.md', 'docs/configuration-and-reconstruction.md', 'docs/manual-assembly-use.md')
    if ($taskBinary.Entries.Count -ne $taskExpectedBinary.Count) { throw 'Unexpected precompiled archive entries.' }
    foreach ($taskName in $taskExpectedBinary) {
        if ($null -eq $taskBinary.GetEntry($taskName)) { throw "Missing precompiled archive entry: $taskName" }
    }
}
finally { $taskBinary.Dispose() }
$taskLines = [IO.File]::ReadAllLines((Join-Path $taskPackages 'SHA256SUMS.txt'))
if ($taskLines.Count -ne 3) { throw 'Expected exactly three release-asset checksums.' }
foreach ($taskLine in $taskLines) {
    if ($taskLine -notmatch '^([a-f0-9]{64})  (Inheto[.-][\w.-]+\.(nupkg|snupkg|zip))$') { throw 'Malformed checksum entry.' }
    if ((Get-FileHash -LiteralPath (Join-Path $taskPackages $Matches[2]) -Algorithm SHA256).Hash.ToLowerInvariant() -cne $Matches[1]) {
        throw 'Release-asset checksum mismatch.'
    }
}
Write-Host 'PASS package identity, two public dependencies, exact README/guides, runtime/symbol/archive boundaries, and SHA-256 checksums.'
