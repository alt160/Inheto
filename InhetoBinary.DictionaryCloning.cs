using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace Inheto;

public partial class InhetoBinary
{
    private static readonly ConcurrentDictionary<Type, Func<IDictionary, object>> knownDictionaryCloners = new();

    /// <summary>
    /// Clones the supported BCL dictionary families without erasing their comparer or concrete collection type.<br/>
    /// Uses one cached closed generic delegate per CLR dictionary type; arbitrary custom dictionary classes
    /// remain on the existing fallback rather than acquiring an inferred copy/constructor contract.<br/>
    /// </summary>
    private static bool TryCloneKnownDictionary(IDictionary source, out object? clone)
    {
        var type = source.GetType();
        var definition = type.GetGenericTypeDefinition();
        if (definition != typeof(Dictionary<,>) && definition != typeof(SortedDictionary<,>) &&
            definition != typeof(SortedList<,>) && definition != typeof(ConcurrentDictionary<,>))
        {
            clone = null;
            return false;
        }
        var cloner = knownDictionaryCloners.GetOrAdd(type, static dictionaryType =>
            typeof(InhetoBinary).GetMethod(nameof(CloneKnownDictionary), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(dictionaryType.GetGenericArguments())
                .CreateDelegate<Func<IDictionary, object>>());
        clone = cloner(source);
        return true;
    }

    /// <summary>
    /// Uses comparer-aware typed copy constructors, preserving the existing shallow-clone boundary.<br/>
    /// Entry values are not deep-cloned; dictionary mutation remains isolated while comparer identity is retained.<br/>
    /// Typed BCL copies avoid the previous non-generic DictionaryEntry boxing loop.<br/>
    /// </summary>
    private static object CloneKnownDictionary<TKey, TValue>(IDictionary source) where TKey : notnull => source switch
    {
        Dictionary<TKey, TValue> value => new Dictionary<TKey, TValue>(value, value.Comparer),
        SortedDictionary<TKey, TValue> value => new SortedDictionary<TKey, TValue>(value, value.Comparer),
        SortedList<TKey, TValue> value => new SortedList<TKey, TValue>(value, value.Comparer),
        ConcurrentDictionary<TKey, TValue> value => new ConcurrentDictionary<TKey, TValue>(value, value.Comparer),
        _ => throw new InvalidOperationException("Unexpected dictionary type in the cached clone dispatcher.")
    };
}
