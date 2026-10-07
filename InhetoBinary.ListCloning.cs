using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Inheto;

public partial class InhetoBinary
{
    private static readonly ConcurrentDictionary<Type, Func<IList, object>> knownListCloners = new();

    /// <summary>
    /// Clones exact BCL List and ObservableCollection instances without erasing their concrete family.<br/>
    /// A cached closed generic delegate avoids per-copy reflection, capacity growth and non-generic value-element boxing.<br/>
    /// Custom subclasses retain the existing fallback; no custom constructor or additional-state copy contract is inferred.<br/>
    /// </summary>
    /// <param name="source">Generic IList selected by the existing mutable-value clone dispatcher.<br/></param>
    /// <param name="clone">Independent collection storage with the same shallow element references, when supported.<br/></param>
    /// <returns>True only for the two exact known BCL generic definitions.<br/></returns>
    private static bool TryCloneKnownList(IList source, out object? clone)
    {
        var type = source.GetType();
        var definition = type.GetGenericTypeDefinition();
        if (definition != typeof(List<>) && definition != typeof(ObservableCollection<>))
        {
            clone = null;
            return false;
        }
        var cloner = knownListCloners.GetOrAdd(type, static listType =>
            typeof(InhetoBinary).GetMethod(nameof(CloneKnownList), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(listType.GetGenericArguments())
                .CreateDelegate<Func<IList, object>>());
        clone = cloner(source);
        return true;
    }

    /// <summary>
    /// Uses typed BCL copy constructors, preserving collection order, nulls and shallow reference-element sharing.<br/>
    /// ObservableCollection event subscriptions are not copied and construction does not mutate or notify the source.<br/>
    /// The exact-family gate prevents these constructors from silently slicing custom derived collections.<br/>
    /// </summary>
    /// <typeparam name="TElement">Actual closed collection element type, including nullable values and arrays.<br/></typeparam>
    /// <param name="source">Known exact List or ObservableCollection instance.<br/></param>
    /// <returns>A new mutable collection of the same supported concrete type.<br/></returns>
    private static object CloneKnownList<TElement>(IList source) => source switch
    {
        List<TElement> value => new List<TElement>(value),
        ObservableCollection<TElement> value => new ObservableCollection<TElement>(value),
        _ => throw new InvalidOperationException("Unexpected list type in the cached clone dispatcher.")
    };
}
