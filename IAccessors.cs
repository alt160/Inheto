using System;
using System.Collections.Generic;
using System.Linq;

namespace Inheto
{
    /// <summary>
    /// Describes compiled property or field access, including the by-reference assignment path required for value-type instances.<br/>
    /// </summary>
    public interface IAccessors
    {
        /// <summary>
        /// Invokes the compiled getter on an instance of the declared owner type; returns null when no getter exists.<br/>
        /// </summary>
        /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
        /// <returns>Member value, or null when no getter exists; null can also be a legitimate member value.<br/></returns>
        object? GetValue(object instance);
        /// <summary>
        /// Attempts member assignment, using an in-place boxed-struct path for value-type owners. A false result indicates that no applicable setter or box is available; casts and developer setter exceptions propagate.<br/>
        /// </summary>
        /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        /// <returns>True when the available setter path was processed; false when no applicable setter or struct box exists.<br/></returns>
        bool SetValue(ref object instance, object? value);
        /// <summary>
        /// Invokes the by-reference setter when available. The referenced instance must have the exact declared owner type; this low-level path does not validate the Unsafe.As reinterpretation.<br/>
        /// </summary>
        /// <typeparam name="T">Exact owner type; must match the compiled accessor owner without reinterpretation to a different type.<br/></typeparam>
        /// <param name="instance">Compatible owner instance to read or mutate; a by-reference argument preserves value-type changes.<br/></param>
        /// <param name="value">Input member or scalar value, including null only where the declared type and conversion allow it.<br/></param>
        /// <returns>True when the by-reference setter was invoked; false when it is absent.<br/></returns>
        bool TrySetValueRef<T>(ref T instance, object? value);         // value-types path

        /// <summary>
        /// Gets whether both setter delegates are absent; this describes accessor capability, not deep immutability of the value.<br/>
        /// </summary>
        bool ReadOnly { get; } 
        /// <summary>
        /// Gets the CLR type expected for the member value.<br/>
        /// </summary>
        Type MemberType { get; }
        /// <summary>
        /// Gets the declaring instance type expected by the compiled accessors.<br/>
        /// </summary>
        Type InstanceType { get; }
    }
}

