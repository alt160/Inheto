using Fasterflect;
using System.Collections.Concurrent;
using System.Reflection;

namespace Inheto
{
    public partial class InhetoBinary
    {
        private static readonly ConcurrentDictionary<(RuntimeTypeHandle TypeHandle, MemberTypesEnum MemberTypes), MaterializationMember[]> MaterializationMemberCache = new();
        private object? activeComplexRoot;

        /// <summary>
        /// Returns the immutable member materialization plan for one CLR type and member-selection policy.<br/>
        /// Reflection discovery, property/field combination, path-segment construction, type classification,
        /// and accessor resolution are performed once per shape instead of once per object.<br/>
        /// </summary>
        /// <param name="targetType">The CLR type whose members will be materialized.</param>
        /// <param name="memberTypes">The property/field visibility policy requested by deserialization.</param>
        /// <returns>An ordered plan preserving the established properties-before-fields traversal contract.</returns>
        private static MaterializationMember[] GetMaterializationMembers(Type targetType, MemberTypesEnum memberTypes)
        {
            return MaterializationMemberCache.GetOrAdd(
                (targetType.TypeHandle, memberTypes),
                static (key, resolvedTargetType) => BuildMaterializationMembers(resolvedTargetType, key.MemberTypes),
                targetType);
        }

        /// <summary>
        /// Builds an immutable member materialization plan for a previously unseen CLR shape.<br/>
        /// The plan intentionally preserves the former reflection flags and properties-before-fields order
        /// while eliminating per-object reflection arrays, lists, LINQ iterators, and path-segment strings.<br/>
        /// </summary>
        /// <param name="targetType">The CLR type whose materializable members should be discovered.</param>
        /// <param name="memberTypes">The property/field visibility policy requested by deserialization.</param>
        /// <returns>A compact ordered plan reused by all subsequent instances of the same shape.</returns>
        private static MaterializationMember[] BuildMaterializationMembers(Type targetType, MemberTypesEnum memberTypes)
        {
            IList<PropertyInfo> properties = Array.Empty<PropertyInfo>();
            IList<FieldInfo> fields = Array.Empty<FieldInfo>();

            switch (memberTypes)
            {
                case MemberTypesEnum.PublicPropertiesOnly:
                    properties = targetType.Properties(Flags.InstancePublic);
                    break;
                case MemberTypesEnum.PublicFieldsOnly:
                    fields = targetType.Fields(Flags.InstancePublic);
                    break;
                case MemberTypesEnum.PublicAndPrivateFields:
                    fields = targetType.Fields(Flags.InstancePublic | Flags.InstancePrivate);
                    break;
                case MemberTypesEnum.PublicPropertiesAndFields:
                    properties = targetType.Properties(Flags.InstancePublic);
                    fields = targetType.Fields(Flags.InstancePublic);
                    break;
                case MemberTypesEnum.PrivateFieldsOnly:
                    fields = targetType.Fields(Flags.InstancePrivate);
                    break;
            }

            var members = new MaterializationMember[properties.Count + fields.Count];
            var destination = 0;
            var collectionShape = targetType.IsAssignableTo(typeof(System.Collections.IDictionary)) ||
                targetType.IsAssignableTo(typeof(System.Collections.ICollection)) ||
                InhetoSerializer.IsSetType(targetType);
            var arrayShape = targetType.IsArray;

            for (var index = 0; index < properties.Count; index++)
            {
                // These BCL getters alias elements already handled by collection filling.
                // Mutating Min/Max in place changes an existing set element's sort key;
                // reference-aware navigation now makes their serialized descendants reachable.
                // Restrict the exclusion to the BCL declaration, not user properties with these names.
                var property = properties[index];
                if (property.Name is "Min" or "Max" &&
                    property.DeclaringType is { IsGenericType: true } declaring &&
                    declaring.GetGenericTypeDefinition() == typeof(SortedSet<>))
                    continue;
                var member = (PropField)properties[index];
                if (ShouldSkipMaterializationMember(collectionShape, arrayShape, member.Name))
                    continue;
                members[destination++] = CreateMaterializationMember(targetType, member, memberTypes);
            }

            for (var index = 0; index < fields.Count; index++)
            {
                var member = (PropField)fields[index];
                if (ShouldSkipMaterializationMember(collectionShape, arrayShape, member.Name))
                    continue;
                members[destination++] = CreateMaterializationMember(targetType, member, memberTypes);
            }

            if (destination != members.Length)
                Array.Resize(ref members, destination);

            return members;
        }

        /// <summary>
        /// Identifies framework collection and array plumbing that is not part of the serialized object graph.<br/>
        /// Filtering occurs before type classification and accessor creation so abstract comparer contracts and
        /// other implementation-only members retain the established skip-before-resolution behavior.<br/>
        /// </summary>
        /// <param name="collectionShape">Whether the declaring type is a dictionary, collection, or set.</param>
        /// <param name="arrayShape">Whether the declaring type is an array.</param>
        /// <param name="memberName">The reflected property or field name.</param>
        /// <returns><see langword="true"/> when the member must not participate in materialization.</returns>
        private static bool ShouldSkipMaterializationMember(bool collectionShape, bool arrayShape, string memberName)
        {
            if (collectionShape && memberName is
                "Keys" or
                "IsEmpty" or
                "AllKeys" or
                "Values" or
                "Comparer" or
                "Item" or
                "SyncRoot" or
                "IsReadOnly" or
                "IsFixedSize" or
                "IsSynchronized" or
                "Capacity")
            {
                return true;
            }

            return arrayShape && memberName is "Rank" or "LongLength" or "Length";
        }

        /// <summary>
        /// Compiles one reflected property or field into the values needed by the hot materialization loop.<br/>
        /// All results are immutable and safe to share across concurrent deserializations.<br/>
        /// </summary>
        /// <param name="targetType">The declaring runtime type used to resolve the assignment accessor.</param>
        /// <param name="member">The reflected property-or-field union.</param>
        /// <param name="memberTypes">The member-selection policy used for Inheto type classification.</param>
        /// <returns>A reusable materialization member.</returns>
        private static MaterializationMember CreateMaterializationMember(Type targetType, PropField member, MemberTypesEnum memberTypes)
        {
            var memberType = member.Type;
            return new MaterializationMember(
                member,
                string.Concat(".", member.Name),
                memberType,
                memberType.IsEnum,
                InhetoTypeResolver.GetInhetoTypeInfo(memberType, memberTypes),
                // A selected member has a validated accessor, even when it has no setter.
                // Empty descriptors fail at member.Type above; default array tails are trimmed before publication.
                GetAccessors(targetType, member)!);
        }

        /// <summary>
        /// Materializes a top-level complex CLR object without creating the mutable-reference cache wrapper.<br/>
        /// A root produced by <c>ToObject&lt;T&gt;</c> is consumed exactly once and the per-binary reference cache is
        /// cleared before and after that operation. An operation-local root reference closes back-references
        /// during population without eagerly allocating a root reference-cache entry.<br/>
        /// Nested complex values and mutable collection/reference shapes continue through the existing cache path.<br/>
        /// </summary>
        /// <typeparam name="T">The requested top-level CLR object type.</typeparam>
        /// <param name="propPath">The resolved root property path, normally the empty string.</param>
        /// <param name="options">The prepared deserialization options for this root operation.</param>
        /// <returns>The newly activated and populated complex object, or null when activation declines rehydration.<br/></returns>
        private T? MaterializeComplexRoot<T>(string propPath, DeserializationOptions? options)
        {
            options ??= new DeserializationOptions();

            T? value;
            if (options.TryGetActivator(typeof(T), out var activator))
                value = (T?)activator(CreateActivatorContext(propPath));
            else
                value = Activator.CreateInstance<T>();

            if (value is null)
                return default;

            object boxed = value;
            // Publish only a borrowed operation-local root reference. This avoids creating
            // a reference-cache dictionary for ordinary roots just to support a back-reference.
            object? previousRoot = activeComplexRoot;
            activeComplexRoot = typeof(T).IsValueType ? null : boxed;
            try
            {
                boxed = doPropsAndFields(options, propPath, ref boxed, useCoercion: true);
                return (T?)boxed;
            }
            finally { activeComplexRoot = previousRoot; }
        }

        /// <summary>
        /// Stores the immutable reflection and decoding metadata consumed for one materialized member.<br/>
        /// Instances are created once per CLR type/member-selection pair and then reused without per-object
        /// reflection, type classification, accessor lookup, or path-segment allocation.<br/>
        /// </summary>
        private readonly struct MaterializationMember
        {
            /// <summary>
            /// Initializes a reusable member materialization description.<br/>
            /// </summary>
            public MaterializationMember(
                PropField member,
                string pathSegment,
                Type memberType,
                bool isEnum,
                InhetoTypeInfo typeInfo,
                IAccessors accessors)
            {
                Member = member;
                PathSegment = pathSegment;
                MemberType = memberType;
                IsEnum = isEnum;
                TypeInfo = typeInfo;
                Accessors = accessors;
            }

            public PropField Member { get; }
            public string PathSegment { get; }
            public Type MemberType { get; }
            public bool IsEnum { get; }
            public InhetoTypeInfo TypeInfo { get; }
            public IAccessors Accessors { get; }
        }
    }
}
