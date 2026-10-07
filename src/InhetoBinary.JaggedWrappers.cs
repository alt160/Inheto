using System.Collections.Concurrent;
using System.Reflection;

namespace Inheto;

public partial class InhetoBinary
{
    private static readonly ConcurrentDictionary<Type, Func<NameHeaderEntry, Array, IInhetoObject>> jaggedWrappers = new();

    /// <summary>
    /// Materializes a jagged value and selects the rank-one wrapper from its immediate CLR element type.<br/>
    /// An int[][] therefore becomes InhetoArray1&lt;int[]&gt;, not InhetoArray1&lt;int&gt;.<br/>
    /// Uses the existing array reader and its cache/clone policy, without coercing child arrays to scalar leaves.<br/>
    /// Closed wrapper delegates are cached by CLR array type and never capture readers, entries, or payloads.<br/>
    /// </summary>
    /// <param name="entry">The resolved jagged-array entry.<br/></param>
    /// <param name="options">The effective deserialization options for this operation.<br/></param>
    /// <returns>The existing typed rank-one wrapper owning the materialized array reference.<br/></returns>
    /// <exception cref="InvalidDataException">The declared array unexpectedly produces no value.<br/></exception>
    private IInhetoObject ReadJaggedWrapper(NameHeaderEntry entry, DeserializationOptions options)
    {
        var leaf = entry.Type.ArrayInfo.ElementType;
        Type leafType = InhetoToTypeResolver.Resolve(in leaf) ?? typeof(object);
        var value = ReadArrayOf(leafType, entry.PropPath, options, false)
            ?? throw new InvalidDataException($"Jagged array construction produced no value for '{entry.PropPath}'.");
        var factory = jaggedWrappers.GetOrAdd(value.GetType(), static type =>
            typeof(InhetoBinary).GetMethod(nameof(WrapJagged), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(type.GetElementType()!)
                .CreateDelegate<Func<NameHeaderEntry, Array, IInhetoObject>>());
        return factory(entry, value);
    }

    /// <summary>
    /// Constructs the existing rank-one wrapper around the array already returned by the materializer.<br/>
    /// Performs one reference cast and wrapper allocation, without array copying or per-element reflection.<br/>
    /// </summary>
    /// <typeparam name="T">The immediate array element type, including remaining jagged levels.<br/></typeparam>
    /// <param name="entry">The resolved source entry and borrowed metadata.<br/></param>
    /// <param name="value">The already materialized T[] array.<br/></param>
    /// <returns>A strongly typed rank-one output.<br/></returns>
    private static IInhetoObject WrapJagged<T>(NameHeaderEntry entry, Array value) => new InhetoArray1<T>(entry, (T[])value);
}
