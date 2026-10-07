param(
    [ValidateSet('net8.0', 'net9.0', 'net10.0')] [string] $Framework = 'net8.0',
    [string] $Version = '1.0.1',
    [string] $PackageDirectory = (Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts/packages'),
    [string] $WorkDirectory = (Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts/consumer-validation'),
    [string] $MSBuildPath = '',
    [string] $BaselineLibrary = '')

$ErrorActionPreference = 'Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+$') { throw 'A stable major.minor.patch version is required.' }
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskFeed = [IO.Path]::GetFullPath($PackageDirectory)
$taskWork = [IO.Path]::GetFullPath((Join-Path $WorkDirectory $Framework))
if (-not (Test-Path -LiteralPath (Join-Path $taskFeed "Inheto.$Version.nupkg"))) { throw 'Build and pack the release candidate first.' }
[IO.Directory]::CreateDirectory($taskWork) | Out-Null
# An exact source mapping keeps same-version candidate tests from silently using
# the published package. Dependencies still restore from NuGet.org.
$taskConfig = Join-Path $taskWork 'NuGet.Config'
$taskConfigXml = [xml] '<configuration><packageSources><clear/><add key="candidate" value=""/><add key="nuget.org" value="https://api.nuget.org/v3/index.json"/></packageSources><fallbackPackageFolders><clear/></fallbackPackageFolders><packageSourceMapping><clear/><packageSource key="candidate"><package pattern="Inheto"/></packageSource><packageSource key="nuget.org"><package pattern="*"/></packageSource></packageSourceMapping></configuration>'
$taskConfigXml.configuration.packageSources.add[0].SetAttribute('value', $taskFeed)
$taskConfigXml.Save($taskConfig)
Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz') Build and verify independent Inheto $Version consumers on $Framework."
$taskSuites = @('Inheto.ReadmeSmoke', 'Inheto.ActivationReview', 'Inheto.LiteralPathReview',
    'Inheto.ReleaseBoundarySmoke', 'Inheto.CollectionReview')
foreach ($taskSuite in $taskSuites) {
    $taskProject = Join-Path $taskRoot "tests/$taskSuite/$taskSuite.csproj"
    $taskOutput = Join-Path $taskWork "$taskSuite/bin/"
    $taskIntermediate = Join-Path $taskWork "$taskSuite/obj/"
    $taskLog = Join-Path $taskWork "$taskSuite-build.log"
    & (Join-Path $PSScriptRoot 'Backup-ChangedSources.ps1') -RepositoryRoot $taskRoot
    $taskProperties = @("/p:SmokeFramework=$Framework", "/p:InhetoPackageVersion=$Version", "/p:PackageFeed=$taskFeed",
        "/p:RestorePackagesPath=$taskWork/packages", "/p:BaseIntermediateOutputPath=$taskIntermediate", "/p:OutputPath=$taskOutput",
        "/p:RestoreConfigFile=$taskConfig", '/p:DisableImplicitLibraryPacksFolder=true',
        '/p:DisableImplicitNuGetFallbackFolder=true',
        '/p:AppendTargetFrameworkToOutputPath=false', '/p:TreatWarningsAsErrors=true')
    if ($MSBuildPath) {
        & $MSBuildPath $taskProject /t:Restore,Build /p:Configuration=Release /p:Platform=AnyCPU /m:1 /v:minimal /fl "/flp:logfile=$taskLog;verbosity=normal" @taskProperties
    }
    else {
        & dotnet build $taskProject --configuration Release --verbosity minimal @taskProperties "/flp:logfile=$taskLog;verbosity=normal"
    }
    if ($LASTEXITCODE -ne 0) { throw "$taskSuite build failed on $Framework." }
    $taskMetadata = Get-Content -LiteralPath (Join-Path $taskWork "packages/inheto/$Version/.nupkg.metadata") -Raw | ConvertFrom-Json
    if (-not [string]::Equals([IO.Path]::GetFullPath($taskMetadata.source), $taskFeed, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Consumer package was not restored from the candidate feed.'
    }
    $taskLibrary = Join-Path $taskOutput 'InhetoSerializer.dll'
    $taskZip = [IO.Compression.ZipFile]::OpenRead((Join-Path $taskFeed "Inheto.$Version.nupkg"))
    try {
        $taskInput = $taskZip.GetEntry('lib/net8.0/InhetoSerializer.dll').Open()
        try { $taskHash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($taskInput)) }
        finally { $taskInput.Dispose() }
        if ((Get-FileHash -LiteralPath $taskLibrary -Algorithm SHA256).Hash -cne $taskHash) { throw 'Consumer did not load the exact packaged DLL.' }
    }
    finally { $taskZip.Dispose() }
    $taskAssembly = Join-Path $taskOutput "$taskSuite.dll"
    $taskModes = @(switch ($taskSuite) {
        'Inheto.ReadmeSmoke' { ,@((Join-Path $taskRoot 'README.md'), (Join-Path $taskRoot "tests/$taskSuite/Program.cs")) }
        'Inheto.ActivationReview' { ,@('--verify', (Join-Path $taskRoot 'docs/configuration-and-reconstruction.md'), (Join-Path $taskRoot "tests/$taskSuite/Program.cs")) }
        'Inheto.ReleaseBoundarySmoke' {
            ,@('--verify-boundary')
            ,@('--verify-unsupported')
            ,@('--verify-symbols', $taskFeed, $Version)
            if ($BaselineLibrary) { ,@('--compare-il', [IO.Path]::GetFullPath($BaselineLibrary)) }
        }
        default { ,@() }
    })
    $taskRun = 0
    foreach ($taskMode in $taskModes) {
        $taskRun++
        Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz') Run $taskSuite $Framework mode $taskRun."
        & dotnet $taskAssembly @taskMode 2>&1 | Tee-Object -FilePath (Join-Path $taskWork "$taskSuite-run-$taskRun.log")
        if ($LASTEXITCODE -ne 0) { throw "$taskSuite verification failed on $Framework." }
    }
}
Write-Host "PASS all independent package-consumer suites on $Framework."
