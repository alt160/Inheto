using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Inheto
{
    /// <summary>
    /// Assigns a member through a reference to its declaring instance, preserving value-type mutations.<br/>
    /// </summary>
    /// <typeparam name="TInstance">Exact CLR owner type used by the compiled accessors.<br/></typeparam>
    /// <typeparam name="TMember">Exact CLR member value type used by the compiled delegates.<br/></typeparam>
    /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
    /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
    public delegate void RefSetter<TInstance, TMember>(ref TInstance instance, TMember value);
    /// <summary>
    /// Stores compiled member-access delegates for one exact instance/member type pair. Configure before use; mutable delegate registration is not synchronized.<br/>
    /// </summary>
    /// <typeparam name="TInstance">Exact CLR owner type used by the compiled accessors.<br/></typeparam>
    /// <typeparam name="TMember">Exact CLR member value type used by the compiled delegates.<br/></typeparam>
    public class Accessors<TInstance, TMember> : IAccessors
    {
        // PROPERTIES
        /// <summary>
        /// Gets or sets the compiled member getter; null means no getter is available.<br/>
        /// </summary>
        public Func<TInstance, TMember>? Getter { get; set; }
        /// <summary>
        /// Gets or sets the compiled by-value instance setter. Struct owners require SetterByRef to preserve mutation.<br/>
        /// </summary>
        public Action<TInstance, TMember>? Setter { get; set; }
        /// <summary>
        /// Gets or sets the compiled by-reference instance setter used to mutate a value-type owner in place.<br/>
        /// </summary>
        public RefSetter<TInstance, TMember>? SetterByRef { get; set; }    // for structs
                                                                            // define once


        // METHODS (IAccessors implementation)
        /// <summary>
        /// Invokes the compiled getter on an instance of the declared owner type; returns null when no getter exists.<br/>
        /// </summary>
        /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
        /// <returns>Member value, or null when no getter exists; null can also be a legitimate member value.<br/></returns>
        public object? GetValue(object instance)
        {
            if (Getter == null) return null;
            // For performance, cast once to TInstance
            var typedInstance = (TInstance)instance;
            return Getter.Invoke(typedInstance);
        }

        /// <summary>
        /// Attempts member assignment, using an in-place boxed-struct path for value-type owners. A false result indicates that no applicable setter or box is available; casts and developer setter exceptions propagate.<br/>
        /// </summary>
        /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        /// <returns>True when the available setter path was processed; false when no applicable setter or struct box exists.<br/></returns>
        public bool SetValue(ref object instance, object? value)
        {
            // null or no setter
            if (Setter == null && SetterByRef is null) return false;

            var t = typeof(TInstance);

            if (t.IsValueType)
            {
                // Handle boxed value type directly
                if (instance is null)
                    throw new ArgumentNullException(nameof(instance));

                // Call via AccessorInvoker to mutate the box itself
                AccessorInvoker.SetValueForValueType(this, t, ref instance, value);
                return true;
            }

            // reference types (normal path)
            var typedInstance = (TInstance)instance;
            var typedValue = (TMember)value!;
            Setter?.Invoke(typedInstance, typedValue);
            return true;
        }

        /// <summary>
        /// Invokes the by-reference setter when available. The referenced instance must have the exact declared owner type; this low-level path does not validate the Unsafe.As reinterpretation.<br/>
        /// </summary>
        /// <typeparam name="T">Exact owner type; must match the compiled accessor owner without reinterpretation to a different type.<br/></typeparam>
        /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        /// <returns>True when the by-reference setter was invoked; false when it is absent.<br/></returns>
        public bool TrySetValueRef<T>(ref T instance, object? value)
        {
            if (SetterByRef is null) return false;                // not a struct or no setter
            // reinterpret ref T as ref TInstance without copying
            ref TInstance inst = ref Unsafe.As<T, TInstance>(ref instance);
            SetterByRef(ref inst, (TMember)value!);
            return true;
        }

        /// <summary>
        /// Gets whether both setter delegates are absent; this describes accessor capability, not deep immutability of the value.<br/>
        /// </summary>
        public bool ReadOnly => Setter is null && SetterByRef is null;

        /// <summary>
        /// Gets the CLR type expected for the member value.<br/>
        /// </summary>
        public Type MemberType => typeof(TMember);
        /// <summary>
        /// Gets the declaring instance type expected by the compiled accessors.<br/>
        /// </summary>
        public Type InstanceType => typeof(TInstance);

    }

    /// <summary>
    /// Assigns a member through a boxed instance passed by reference, allowing the box or its contained value to be updated.<br/>
    /// </summary>
    /// <param name="acc">Compiled accessor whose declared owner and member types match this operation.<br/></param>
    /// <param name="boxedInstance">Existing non-null box containing the exact struct owner type; assignment mutates that box in place.<br/></param>
    /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
    public  delegate void BoxedRefSetter(IAccessors acc, ref object boxedInstance, object? value);

    /// <summary>
    /// Dispatches member assignment to the concrete value type so an existing box can be mutated without losing struct updates.<br/>
    /// </summary>
    public static class AccessorInvoker
    {
        private static readonly ConcurrentDictionary<Type, BoxedRefSetter> Cache = new();

        /// <summary>
        /// Dispatches assignment to an exact struct type, preserving changes to the existing value or boxed value. The supplied accessor must match that owner type.<br/>
        /// </summary>
        /// <param name="acc">Compiled accessor whose declared owner and member types match this operation.<br/></param>
        /// <param name="t">CLR type whose supported representation or dictionary arguments are inspected.<br/></param>
        /// <param name="boxedInstance">Existing non-null box containing the exact struct owner type; assignment mutates that box in place.<br/></param>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        public static void SetValueForValueType(IAccessors acc, Type t, ref object boxedInstance, object? value)
        {
            var inv = Cache.GetOrAdd(t, Build);
            inv(acc, ref boxedInstance, value);
        }

        /// <summary>
        /// Dispatches assignment to an exact struct type, preserving changes to the existing value or boxed value. The supplied accessor must match that owner type.<br/>
        /// </summary>
        /// <typeparam name="T">Exact struct owner type preserved by reference during assignment.<br/></typeparam>
        /// <param name="acc">Compiled accessor whose declared owner and member types match this operation.<br/></param>
        /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        public static void SetValueForValueType<T>(IAccessors acc, ref T instance, object? value) where T : struct
        {
            acc.TrySetValueRef(ref instance, value);
        }

        private static BoxedRefSetter Build(Type t)
        {
            var mi = typeof(AccessorInvoker).GetMethod(nameof(InvokeGeneric),
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                     .MakeGenericMethod(t);
            return (BoxedRefSetter)mi.CreateDelegate(typeof(BoxedRefSetter));
        }

        private static void InvokeGeneric<T>(IAccessors acc, ref object boxedInstance, object? value) where T : struct
        {
            ref T r = ref Unsafe.Unbox<T>(boxedInstance);
            acc.TrySetValueRef(ref r, value);
        }
    }

    /// <summary>
    /// Contains the compiled accessor factory used internally by reflection-based member dispatch.<br/>
    /// </summary>
    public static class AccessorUtils
    {
        /// <summary>
        /// Builds reusable compiled accessors for a selected field or property.<br/>
        /// Readonly fields retain their getter but have no assignment accessor; this does not restrict private-member discovery.<br/>
        /// Writable fields retain the existing class or by-ref struct setter, and properties retain their available accessors.<br/>
        /// </summary>
        /// <typeparam name="TInstance">Declaring runtime instance type.<br/></typeparam>
        /// <typeparam name="TMember">Exact reflected member type.<br/></typeparam>
        /// <param name="member">Member already selected by the caller's visibility policy.<br/></param>
        /// <returns>The accessor pair or getter-only accessor to retain in the existing metadata cache.<br/></returns>
        private static Accessors<TInstance, TMember> CreateAccessors<TInstance, TMember>(MemberInfo member)
        {
            var result = new Accessors<TInstance, TMember>();

            if (member is FieldInfo f)
            {
                // getter
                var gi = Expression.Parameter(typeof(TInstance), "i");
                result.Getter = Expression.Lambda<Func<TInstance, TMember>>(Expression.Field(gi, f), gi).Compile();

                // Read capability does not require assignment capability. Keep this decision in cold setup.
                if (f.IsInitOnly) return result;

                // setter by-ref for structs, normal setter for classes
                var vi = Expression.Parameter(typeof(TMember), "v");
                if (typeof(TInstance).IsValueType)
                {
                    var gri = Expression.Parameter(typeof(TInstance).MakeByRefType(), "r");
                    var fld = Expression.Field(gri, f);
                    result.SetterByRef = Expression.Lambda<RefSetter<TInstance, TMember>>(
                                          Expression.Assign(fld, vi), gri, vi).Compile();
                }
                else
                {
                    var fld = Expression.Field(gi, f);
                    result.Setter = Expression.Lambda<Action<TInstance, TMember>>(
                                     Expression.Assign(fld, vi), gi, vi).Compile();
                }
                return result;
            }
            if (member is PropertyInfo p)
            {
                if (p.GetIndexParameters().Length != 0) throw new NotSupportedException("Indexers not supported.");

                // getter
                if (p.CanRead)
                {
                    var gi = Expression.Parameter(typeof(TInstance), "i");
                    result.Getter = Expression.Lambda<Func<TInstance, TMember>>(Expression.Property(gi, p), gi).Compile();
                }

                // setter
                if (p.CanWrite)
                {
                    var vi = Expression.Parameter(typeof(TMember), "v");
                    var sm = p.GetSetMethod(true)!;

                    if (typeof(TInstance).IsValueType)
                    {
                        var gri = Expression.Parameter(typeof(TInstance).MakeByRefType(), "r");
                        result.SetterByRef = Expression.Lambda<RefSetter<TInstance, TMember>>(
                                              Expression.Call(gri, sm, vi), gri, vi).Compile();
                    }
                    else
                    {
                        var gi = Expression.Parameter(typeof(TInstance), "i");
                        result.Setter = Expression.Lambda<Action<TInstance, TMember>>(
                                         Expression.Call(gi, sm, vi), gi, vi).Compile();
                    }
                }
                return result;
            }

            throw new ArgumentException("Member must be FieldInfo or PropertyInfo.", nameof(member));
        }

    }
}




