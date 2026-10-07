# Using the precompiled assembly archive

The recommended installation is `dotnet add package Inheto`. NuGet resolves dependencies and selects the compatible .NET 8 library asset for .NET 8 and later applications.

The release ZIP is a precompiled **library**, not an executable or a dependency-free application. It includes `InhetoSerializer.dll`, XML documentation, portable debug symbols, the license, README, and usage guides. It does not bundle assemblies licensed and published separately by other packages.

For manual assembly references, obtain the compatible assemblies from **BufferStream 1.0.2** and **fasterflect 3.0.0** as well, and ensure your application deploys/resolves them. Follow those packages' licenses. Prefer NuGet unless your deployment process specifically requires manual references.

`SHA256SUMS.txt` covers the NuGet package, symbols package, and precompiled ZIP. GitHub artifact attestations accompany authorized automated releases.
