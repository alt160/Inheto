# Release checklist

## Identity and boundaries

- NuGet package: **Inheto**; first release **1.0.0**.
- GitHub repository: **alt160/Inheto**; public author: **alt160**; license: **Apache-2.0**.
- Existing assembly: **InhetoSerializer**; namespace: **Inheto**. Package naming does not rename the assembly or break its existing friend-assembly contracts.
- One `net8.0` library asset; independent consumers validate the same bytes on .NET 8, 9, and 10. Later compatible .NET releases can use the asset, but are not claimed as tested in advance.
- Public runtime dependencies: **BufferStream 1.0.2** and **fasterflect 3.0.0**. SourceLink is build-only.
- Maintained tests ship as repository source, not library APIs or package content. Local histories, old probes, drafts, and legacy transformation helpers are excluded.

## Local validation (PowerShell 7)

Run from the repository root, with Git and the .NET 10 SDK used by CI (the resulting library still targets .NET 8). These commands build and inspect local artifacts only; they do not push or publish:

```powershell
./scripts/Backup-ChangedSources.ps1
dotnet build InhetoSerializer.csproj -c Release /p:TreatWarningsAsErrors=true
dotnet pack InhetoSerializer.csproj -c Release --no-build --no-restore -o artifacts/packages
./scripts/New-ReleaseAssets.ps1
./scripts/Test-ReleaseAssets.ps1
./scripts/Test-PackageConsumers.ps1 -Framework net8.0
./scripts/Test-PackageConsumers.ps1 -Framework net9.0
./scripts/Test-PackageConsumers.ps1 -Framework net10.0
```

The consumer script snapshots changed sources immediately before each compile, uses isolated intermediate/output/package-cache directories, and checks that every consumer loads the exact packaged DLL. Collection scenarios execute in separate processes and take several minutes. Runtime suites cover the README and guides, activation, literal/shared/circular paths, collection conversions, public diagnostics, varints, and caller-owned transformations.

For this Windows development workspace, pass the configured Visual Studio MSBuild path through `-MSBuildPath` and place `-WorkDirectory` on the dedicated test volume. Build is incremental; Rebuild requires a preceding targeted cleanup of only the selected projects' `bin`/`obj` directories after snapshots. Do not erase unrelated work. `New-ReleaseAssets.ps1` refuses to overwrite a ZIP; use a fresh candidate directory when revising local release files.

## Before the first authorized publication

- [ ] Confirm the GitHub repository and NuGet package ID are available/owned by the intended accounts.
- [ ] Audit the initial Git file list; do not upload local histories, private drafts, probes, secrets, or generated outputs.
- [ ] Create the GitHub **release** environment; choose appropriate protection/approval settings.
- [ ] Configure NuGet trusted publishing under policy creator **iqueue**, GitHub owner **alt160**, repository **Inheto**, workflow **release.yml**, environment **release**, and package glob **Inheto**. A policy for another repository is not interchangeable simply by adding a package glob.
- [ ] Enable private vulnerability reporting, then configure main-branch protection with required validation checks and protection from deletion/force pushes as appropriate.
- [ ] Review every validation result; distinguish local Windows evidence from untested platforms or future runtimes.
- [ ] Only after publication is authorized, push the reviewed source and matching `v1.0.0` tag.

## Automated release contract

`validate.yml` builds and packs **one** candidate, creates the precompiled ZIP and SHA-256 checksums, then supplies those exact assets to the .NET 8/9/10 consumer jobs. Warnings fail the build. CI runs on Windows; this is not a Linux/macOS compatibility certification.

`release.yml` accepts only a stable version tag matching the project. It waits for the reusable validation workflow, then enters the `release` environment. Only this final job has publication/OIDC permissions. It rechecks the immutable candidate, creates artifact attestations, obtains a short-lived NuGet credential using `iqueue`, publishes the package/symbols, and creates a GitHub release containing the package, symbols, ZIP, and checksums.

Do not move an already published tag or reuse a NuGet version for changed bytes. A rerun may finish a failed publication step, but must not blindly replace release assets. For code or documentation changes, bump the project version, validate, and publish a new matching tag. Update the reusable workflow's default version and consumer default package version alongside the project.
