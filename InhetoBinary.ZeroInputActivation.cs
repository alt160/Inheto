using Fasterflect;
using System.Linq.Expressions;

namespace Inheto;

public partial class InhetoBinary
{
    /// <summary>
    /// Compiles zero-input construction metadata without invoking application factories or singleton getters during cache discovery.<br/>
    /// Public parameterless construction retains its direct cached delegate. Other candidates follow the original property/field/any-visibility parameterless-method order.<br/>
    /// First use returns its already-produced exact-type result and caches the successful factory, not the instance. Warm calls invoke that factory once with no candidate list creation.<br/>
    /// An all-invalid discovery retains the former null fallback; developer options/activators are deliberately absent from this intrinsic cache.<br/>
    /// </summary>
    /// <param name="type">Application-selected or policy-approved CLR Type whose intrinsic zero-input construction is compiled.<br/></param>
    /// <returns>A cached construction delegate whose successful getter/factory executes once per requested result.<br/></returns>
    private static Func<DeserializationOptions.ActivatorContext, object?> BuildZeroInputActivator(Type type)
    {
        var constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor is not null)
        {
            var create = Expression.Lambda<Func<object?>>(Expression.Convert(Expression.New(constructor), typeof(object))).Compile();
            return _ => create();
        }

        var candidates = new List<Func<object?>>();
        foreach (var property in type.Properties(Flags.StaticPublic))
        {
            var getter = property.GetGetMethod();
            if (getter is null || getter.GetParameters().Length != 0 || !type.IsAssignableTo(property.PropertyType)) continue;
            candidates.Add(Expression.Lambda<Func<object?>>(Expression.Convert(Expression.Call(null, getter), typeof(object))).Compile());
        }
        foreach (var field in type.Fields(Flags.StaticPublic))
        {
            if (!type.IsAssignableTo(field.FieldType)) continue;
            candidates.Add(Expression.Lambda<Func<object?>>(Expression.Convert(Expression.Field(null, field), typeof(object))).Compile());
        }
        foreach (var method in type.Methods(Flags.StaticAnyVisibility))
        {
            if (method.ContainsGenericParameters || method.GetParameters().Length != 0 || !type.IsAssignableTo(method.ReturnType)) continue;
            candidates.Add(Expression.Lambda<Func<object?>>(Expression.Convert(Expression.Call(null, method), typeof(object))).Compile());
        }
        if (candidates.Count == 0) return static _ => null;
        Func<object?>[] factories = candidates.ToArray();
        Func<object?>? selected = null;
        return _ =>
        {
            var cached = Volatile.Read(ref selected);
            if (cached is not null) return cached();
            foreach (var factory in factories)
            {
                object? value = factory();
                if (value is null || value.GetType() != type) continue;
                Interlocked.CompareExchange(ref selected, factory, null);
                return value;
            }
            Interlocked.CompareExchange(ref selected, static () => null, null);
            return null;
        };
    }
}
