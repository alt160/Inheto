using System.IO.Compression;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Buffers.Binary;
using System.Security.Cryptography;

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
        int generated = 0;
        foreach (DocumentHandle handle in metadata.Documents)
        {
            string path = metadata.GetString(metadata.GetDocument(handle).Name);
            if (!path.StartsWith("/_/", StringComparison.Ordinal) || path.Contains('\\'))
                throw new InvalidOperationException("Debug symbols expose a non-normalized/local source path: " + path);
            if (path.StartsWith("/_/obj/", StringComparison.Ordinal))
            {
                VerifyEmbeddedGeneratedDocument(metadata, handle);
                generated++;
            }
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
        if (documents == 0 || generated == 0 || mappings != 1) throw new InvalidOperationException("Missing or ambiguous SourceLink/debug source inventory.");
        Console.WriteLine($"PASS portable symbols: {documents} normalized source documents; {generated} checksum-verified embedded generated sources; SourceLink revision {commit}; no host-specific source paths.");
    }

    /// <summary>Verifies that a generated source document is embedded with the exact compiler-recorded checksum rather than mapped to an unavailable repository file.<br/></summary>
    /// <param name="metadata">Portable-PDB metadata from the actual candidate symbols package.<br/></param>
    /// <param name="documentHandle">A normalized generated document under the build-only obj path.<br/></param>
    private static void VerifyEmbeddedGeneratedDocument(MetadataReader metadata, DocumentHandle documentHandle)
    {
        Guid embeddedKind = new("0E8A571B-6926-466E-B4AD-8AB04611F5FE");
        Guid sha256Kind = new("8829D00F-11B8-4213-878B-770E8597AC16");
        Document document = metadata.GetDocument(documentHandle);
        byte[]? source = null;
        foreach (CustomDebugInformationHandle handle in metadata.GetCustomDebugInformation(documentHandle))
        {
            CustomDebugInformation information = metadata.GetCustomDebugInformation(handle);
            if (metadata.GetGuid(information.Kind) != embeddedKind) continue;
            if (source is not null) throw new InvalidOperationException("Duplicated embedded generated source.");
            byte[] blob = metadata.GetBlobBytes(information.Value);
            if (blob.Length < sizeof(int)) throw new InvalidOperationException("Truncated embedded generated source.");
            int size = BinaryPrimitives.ReadInt32LittleEndian(blob);
            if (size < 0 || size > 2 * 1024 * 1024) throw new InvalidOperationException("Unexpected embedded generated-source size.");
            if (size == 0) source = blob[sizeof(int)..];
            else
            {
                using var input = new MemoryStream(blob, sizeof(int), blob.Length - sizeof(int), false);
                using var inflater = new DeflateStream(input, CompressionMode.Decompress);
                using var output = new MemoryStream();
                inflater.CopyTo(output);
                if (output.Length != size) throw new InvalidOperationException("Embedded generated-source size mismatch.");
                source = output.ToArray();
            }
        }
        if (source is null || metadata.GetGuid(document.HashAlgorithm) != sha256Kind ||
            !SHA256.HashData(source).AsSpan().SequenceEqual(metadata.GetBlobBytes(document.Hash)))
            throw new InvalidOperationException("Generated source is missing or differs from its portable-PDB checksum.");
    }
}
