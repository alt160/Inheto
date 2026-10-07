using System.IO.Compression;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;

/// <summary>
/// Validates release debug symbols through the independent package consumer, never through a production API.<br/>
/// Rejects local source-path leakage and SourceLink mappings that do not name this package's source revision.<br/>
/// </summary>
public static class PackageMetadataRegression
{
    /// <summary>
    /// Reads the published-candidate symbols package in memory and checks its portable-PDB SourceLink information.<br/>
    /// Confirms revision identity using the exact production assembly already loaded by this consumer.<br/>
    /// Does not fetch source, activate stored types, or alter files.<br/>
    /// </summary>
    /// <param name="library">The production assembly resolved from the actual NuGet package.<br/></param>
    /// <param name="packageDirectory">Absolute or relative directory containing the candidate symbols package.<br/></param>
    /// <param name="version">The stable version identifying that candidate.<br/></param>
    public static void VerifySymbols(Assembly library, string packageDirectory, string version)
    {
        string informational = library.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? throw new InvalidOperationException("Missing source-revision assembly metadata.");
        string commit = informational[(informational.LastIndexOf('+') + 1)..];
        if (commit.Length != 40 || !commit.All(character => char.IsAsciiHexDigit(character)))
            throw new InvalidOperationException("Production assembly does not identify a full Git source revision.");
        using var symbols = ZipFile.OpenRead(Path.Combine(packageDirectory, $"Inheto.{version}.snupkg"));
        using var input = (symbols.GetEntry("lib/net8.0/InhetoSerializer.pdb")
            ?? throw new InvalidOperationException("Missing production portable PDB.")).Open();
        using var bytes = new MemoryStream();
        input.CopyTo(bytes);
        bytes.Position = 0;
        using var provider = MetadataReaderProvider.FromPortablePdbStream(bytes);
        MetadataReader metadata = provider.GetMetadataReader();
        int documents = 0;
        foreach (DocumentHandle handle in metadata.Documents)
        {
            string path = metadata.GetString(metadata.GetDocument(handle).Name);
            if (!path.StartsWith("/_/", StringComparison.Ordinal) || path.Contains('\\'))
                throw new InvalidOperationException("Debug symbols expose a non-normalized/local source path: " + path);
            documents++;
        }
        Guid sourceLinkKind = new("CC110556-A091-4D38-9FEC-25AB9A351A6A");
        int mappings = 0;
        foreach (CustomDebugInformationHandle handle in metadata.CustomDebugInformation)
        {
            CustomDebugInformation information = metadata.GetCustomDebugInformation(handle);
            if (metadata.GetGuid(information.Kind) != sourceLinkKind) continue;
            using JsonDocument sourceLink = JsonDocument.Parse(metadata.GetBlobBytes(information.Value));
            foreach (JsonProperty mapping in sourceLink.RootElement.GetProperty("documents").EnumerateObject())
            {
                if (!mapping.Name.StartsWith("/_/", StringComparison.Ordinal) ||
                    mapping.Value.GetString() != $"https://raw.githubusercontent.com/alt160/Inheto/{commit}/*")
                    throw new InvalidOperationException("Unexpected source-revision SourceLink mapping.");
                mappings++;
            }
        }
        if (documents == 0 || mappings != 1) throw new InvalidOperationException("Missing or ambiguous SourceLink/debug source inventory.");
        Console.WriteLine($"PASS portable symbols: {documents} normalized source documents; SourceLink revision {commit}; no host-specific source paths.");
    }
}
