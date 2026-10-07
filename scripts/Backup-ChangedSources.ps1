param([string] $RepositoryRoot = (Split-Path $PSScriptRoot -Parent))

$ErrorActionPreference = 'Stop'
$taskRoot = [IO.Path]::GetFullPath($RepositoryRoot)
$taskPrefix = $taskRoot + [IO.Path]::DirectorySeparatorChar
$taskHistory = Join-Path $taskRoot '.code-history'
$taskStamp = Get-Date -Format 'yyyyMMdd-HHmmssfff'
Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz') Snapshot changed source/config/project files before compilation."
Push-Location $taskRoot
try {
    & git rev-parse --verify HEAD 2>$null | Out-Null
    $taskHasHead = $LASTEXITCODE -eq 0
    $taskChanged = if ($taskHasHead) { @(& git diff HEAD --name-only --diff-filter=ACMRTUXB) } else { @(& git ls-files) }
    if ($LASTEXITCODE -ne 0) { throw 'Cannot enumerate tracked source files.' }
    $taskChanged += @(& git ls-files --others --exclude-standard)
    if ($LASTEXITCODE -ne 0) { throw 'Cannot enumerate untracked source files.' }
    $taskCount = 0
    foreach ($taskRelative in $taskChanged | Sort-Object -Unique) {
        if ($taskRelative -match '(^|/)(bin|obj|artifacts|\.code-history|\.vshistory)/') { continue }
        if ($taskRelative -notmatch '\.(cs|vb|csproj|vbproj|props|targets|sln|slnx|json|ya?ml|ps1)$' -and
            $taskRelative -notin @('.gitignore', '.gitattributes')) { continue }
        $taskSource = [IO.Path]::GetFullPath((Join-Path $taskRoot $taskRelative))
        if (-not $taskSource.StartsWith($taskPrefix, [StringComparison]::OrdinalIgnoreCase)) { throw 'Unsafe snapshot source.' }
        if (-not (Test-Path -LiteralPath $taskSource -PathType Leaf)) { continue }
        $taskDirectory = Join-Path $taskHistory ([IO.Path]::GetDirectoryName($taskRelative))
        [IO.Directory]::CreateDirectory($taskDirectory) | Out-Null
        $taskCopy = Join-Path $taskDirectory ($taskStamp + '_' + [IO.Path]::GetFileName($taskRelative))
        [IO.File]::Copy($taskSource, $taskCopy, $false)
        [IO.File]::SetLastWriteTimeUtc($taskCopy, [DateTime]::UtcNow)
        $taskCount++
    }
    $taskPruned = 0
    if (Test-Path -LiteralPath $taskHistory) {
        $taskFiles = @(Get-ChildItem -LiteralPath $taskHistory -File -Recurse)
        if ($taskFiles.Count -gt 0) {
            $taskCutoff = ($taskFiles | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1).LastWriteTimeUtc.AddHours(-4)
            $taskHistoryPrefix = [IO.Path]::GetFullPath($taskHistory) + [IO.Path]::DirectorySeparatorChar
            foreach ($taskOld in $taskFiles | Where-Object LastWriteTimeUtc -lt $taskCutoff) {
                if (-not [IO.Path]::GetFullPath($taskOld.FullName).StartsWith($taskHistoryPrefix, [StringComparison]::OrdinalIgnoreCase)) {
                    throw 'Unsafe history prune target.'
                }
                Remove-Item -LiteralPath $taskOld.FullName
                $taskPruned++
            }
        }
    }
    Write-Host "Snapshotted $taskCount changed files; pruned $taskPruned expired history files."
}
finally { Pop-Location }
