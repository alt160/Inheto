
namespace Inheto
{
    //====== TYPES ======
    /// <summary>
    /// Configures member selection, exclusions, circular-member rules and custom serializers. Finish registration before use; mutable registrations are not synchronized.<br/>
    /// </summary>
    public class SerializationOptions
    {








        //======  FIELDS  ======
        internal readonly Dictionary<string, Func<object, object>> MemberSerializers;
        internal readonly Dictionary<RuntimeTypeHandle, Func<object, object>> TypeSerializers;
        internal readonly HashSet<string> BinaryMemberSerializers;
        internal readonly HashSet<(RuntimeTypeHandle type, string memberName)> CircularProps;
        internal readonly Dictionary<string, byte[]> SerializedMemberOverrides;







        //======  CONSTRUCTORS  ======
        /// <summary>
        /// Creates empty mutable registrations/exclusions and selects public properties and fields by default.<br/>
        /// </summary>
        public SerializationOptions()
        {
            IncludedMembers = new HashSet<string>();
            ExcludedMembers = new HashSet<string>();
            ExcludedTypes = new HashSet<Type>();
            MemberSerializers = new Dictionary<string, Func<object, object>>();
            TypeSerializers = new Dictionary<RuntimeTypeHandle, Func<object, object>>();
            BinaryMemberSerializers = new HashSet<string>();
            MemberTypes = MemberTypesEnum.PublicPropertiesAndFields;
            CircularProps = new HashSet<(RuntimeTypeHandle type, string memberName)>();
            SerializedMemberOverrides = new Dictionary<string, byte[]>();
        }

        /// <summary>
        /// Adds a property to the list of circular properties to prevent infinite recursion during serialization.
        /// The first instance of the property or field of the given name is stored and any nested instances of the same property or field name within that object are set as duplicate value refs.
        /// </summary>
        /// <param name="objectType">Exact CLR owner type whose matching member token receives circular-reference handling.<br/></param>
        /// <param name="memberName">Encoded member token to register for that owner type.<br/></param>
        public void AddCircularProperty(Type objectType, string memberName)
        {
            CircularProps.Add((objectType.TypeHandle, memberName));
        }
        /// <summary>
        /// Adds a property to the list of circular properties to prevent infinite recursion during serialization.
        /// The first instance of the property or field of the given name is stored and any nested instances of the same property or field name within that object are set as duplicate value refs.
        /// </summary>
        /// <typeparam name="T">Declared object type whose matching member token receives circular-reference handling.<br/></typeparam>
        /// <param name="memberName">Member token to register for the declared type, matching the writer's naming convention.<br/></param>
        public void AddCircularProperty<T>(string memberName)
        {
            CircularProps.Add((typeof(T).TypeHandle, memberName));
        }

        /// <summary>
        /// Removes the circular-reference rule for an exact owner type and member token; an absent rule is a no-op.<br/>
        /// </summary>
        /// <param name="objectType">Exact CLR type whose root materialization or owner registration is requested.<br/></param>
        /// <param name="memberName">Member token/path used by the corresponding registration rule.<br/></param>
        public void RemoveCircularProperty(Type objectType, string memberName)
        {
            CircularProps.Remove((objectType.TypeHandle, memberName));
        }

        /// <summary>
        /// When true, allows serializing objects with different key types in a Dictionary.<br/>
        /// Dictionary entries will be serialized as numbered key-value-pairs, similar to a list.
        /// </summary>
        //======  PROPERTIES  ======
        public bool AllowMixedKeyTypesInDictionaryTypes { get; set; }

        //======  PROPERTIES  ======
        /// <summary>
        /// Contains a list of property names from the object being added or updated which will be excluded from being serialized and stored.<br/>
        /// Useful for preventing values that are dynamic, complex, or unnecessary from being stored.
        /// </summary>
        public HashSet<string> ExcludedMembers { get; }
        /// <summary>
        /// Gets the mutable exact-runtime-type exclusion set used for non-root named values. The writer also recognizes a null entry as an exclusion rule for named null values.<br/>
        /// </summary>
        public HashSet<Type> ExcludedTypes { get; }

        /// <summary>
        /// Gets or sets opt-in rejection of values that have no supported ordinary representation; defaults to false.<br/>
        /// When true, the writer throws NotSupportedException with the full Inheto path and rejected Type for native addresses/handles, executable delegates, selected unboxable members, and stateful structs opaque under MemberTypes.<br/>
        /// Explicit exclusions and custom serializers take precedence; empty markers, ordinary class graphs, supported scalars and collections retain their existing behavior.<br/>
        /// Validation occurs during the ordinary write, not through a second graph scan. This is not a guarantee that an arbitrary graph can be reconstructed into every requested CLR type; getter, enumerator and custom-hook exceptions still propagate.<br/>
        /// After failure, discard the partial writer payload; the next Serialize call starts a fresh operation and may reuse the writer.<br/>
        /// </summary>
        public bool FailOnUnsupportedType { get; set; }

        /// <summary>
        /// When not empty, contains a list of property names from the object being added or updated that should be serialized and stored.<br/>
        /// If this list is empty, all non-excluded properties (see <see cref="ExcludedMembers"/>) of the object are serialized.
        /// </summary>
        public HashSet<string> IncludedMembers { get; }

        /// <summary>
        /// Determines the types of properties of the object to serialize, like public properties only or properties and fields or more.
        /// The default includes public instance properties and fields.<br/>
        /// </summary>
        public MemberTypesEnum MemberTypes { get; set; }








        //======  METHODS  ======
        /// <summary>
        /// Adds a special serializer function for <paramref name="propName"/> to convert the value to a <see cref="string"/><br/>
        /// Useful for complex properties that need their own method to convert to string.<br/>
        /// Non-null text is encoded as UTF-8 without a BOM in the normal byte payload; empty text stays distinct from null.<br/>
        /// Pair with the string-facing AddDeserializer extension overload, or decode UTF-8 explicitly in a byte hook.<br/>
        /// Usually adding a serializer here requires adding a deserializer to the <i>DataObjectDeserializationOptions</i> property of the related DataStore.
        /// </summary>
        //------ Public Methods -----
        public void AddSerializer<ValueType>(string propName, Func<ValueType, string> serializer)
        {
            MemberSerializers[propName] = o => serializer((ValueType)o);
            BinaryMemberSerializers.Remove(propName);
        }

        /// <summary>
        /// Adds a special serializer function for <paramref name="propName"/> to convert the value to a <c>byte[]</c>.<br/>
        /// Useful for complex properties that need its own method to convert to byte array.<br/>
        /// Usually adding a serializer here requires adding a deserializer to the <i>DataObjectDeserializationOptions</i> property of the related DataStore.
        /// </summary>
        public void AddSerializer<ValueType>(string propName, Func<ValueType, byte[]> serializer)
        {
            MemberSerializers[propName] = o => serializer((ValueType)o);
            BinaryMemberSerializers.Add(propName);
        }

        /// <summary>
        /// Adds a special serializer function for any usage of the data type <typeparamref name="ValueType"/>.  <br/>
        /// The <paramref name="serializer"/> function is used to convert the value to a <c>byte[]</c>.<br/>
        /// Useful for complex properties that need its own method to convert to byte array.<br/>
        /// Usually adding a serializer here requires adding a deserializer to the <i>DataObjectDeserializationOptions</i> property of the related DataStore.
        /// </summary>
        /// <remarks>This serializer is only used if there is not a property or field specific serializer.</remarks>
        public void AddTypeSerializer<ValueType>(Func<ValueType, byte[]> serializer)
        {
            TypeSerializers[typeof(ValueType).TypeHandle] = o => serializer((ValueType)o);
        }

        /// <summary>
        /// Adds a special serializer function for any usage of the data type <typeparamref name="ValueType"/>.  <br/>
        /// The <paramref name="serializer"/> function is used to convert the value to a <c>byte[]</c>.<br/>
        /// Useful for complex properties that need its own method to convert to byte array.<br/>
        /// Usually adding a serializer here requires adding a deserializer to the <i>DataObjectDeserializationOptions</i> property of the related DataStore.
        /// </summary>
        /// <remarks>This serializer is only used if there is not a property name or field name specific serializer.</remarks>
        public void AddTypeSerializer<ValueType>(bool includeBaseTypes, Func<ValueType, byte[]> serializer)
        {
            Func<object, object> szv = o => serializer((ValueType)o);
            if (includeBaseTypes)
            {
                TypeSerializers[typeof(ValueType).TypeHandle] = szv;
                var baseType = typeof(ValueType).BaseType;
                while (baseType is not null && baseType != typeof(object))
                {
                    TypeSerializers[baseType.TypeHandle] = szv;
                    baseType = baseType.BaseType;
                }
            }
            else
            {
                TypeSerializers[typeof(ValueType).TypeHandle] = szv;
            }
        }


        /// <summary>
        /// Removes the exact-type or member-specific serializer registration addressed by this overload; an absent registration is a no-op.<br/>
        /// </summary>
        /// <typeparam name="ValueType">Exact CLR type targeted by this registration or lookup.<br/></typeparam>
        public void RemoveSerializer<ValueType>()
        {
            TypeSerializers.Remove(typeof(ValueType).TypeHandle);
        }

        /// <summary>
        /// Removes the exact-type or member-specific serializer registration addressed by this overload; an absent registration is a no-op.<br/>
        /// </summary>
        /// <param name="memberName">Member token/path used by the corresponding registration rule.<br/></param>
        public void RemoveSerializer(string memberName)
        {
            MemberSerializers.Remove(memberName);
            BinaryMemberSerializers.Remove(memberName);
        }

        /// <summary>
        /// Reports whether the effective registered serializer for a property path and runtime type produces binary data.<br/>
        /// Property-path registrations have the same precedence used by normal Inheto serialization; a property serializer that produces text therefore intentionally hides a type-wide binary serializer.<br/>
        /// This method inspects registration metadata only and does not invoke developer code or allocate a converted value.<br/>
        /// </summary>
        /// <param name="valueType">The runtime value type whose type-wide registration should be considered.<br/></param>
        /// <param name="propPath">The optional Inheto property path whose member-specific registration should take precedence.<br/></param>
        /// <returns><c>true</c> when the effective handler is registered to produce <c>byte[]</c>; otherwise <c>false</c>.<br/></returns>
        public bool HasBinarySerializer(Type valueType, string? propPath = null)
        {
            ArgumentNullException.ThrowIfNull(valueType);
            if (!string.IsNullOrEmpty(propPath) && MemberSerializers.ContainsKey(propPath))
                return BinaryMemberSerializers.Contains(propPath);

            return TypeSerializers.ContainsKey(valueType.TypeHandle);
        }

        /// <summary>
        /// Invokes the effective registered binary serializer for one runtime value without constructing an outer <see cref="InhetoBinary"/> envelope.<br/>
        /// Property-path handlers take precedence over type-wide handlers exactly as they do during record serialization, allowing indexing and other binary consumers to reuse the developer's existing Inheto conversion intent.<br/>
        /// The returned array is the handler's own result; this method does not clone it.<br/>
        /// </summary>
        /// <param name="value">The non-null runtime value to convert.<br/></param>
        /// <param name="valueType">The exact runtime type used to resolve a type-wide handler when no property handler applies.<br/></param>
        /// <param name="propPath">The optional Inheto property path used for member-specific handler precedence.<br/></param>
        /// <param name="bytes">Receives the handler-owned binary result when an effective binary serializer exists.<br/></param>
        /// <returns><c>true</c> when a binary handler was found and returned a non-null array; otherwise <c>false</c>.<br/></returns>
        public bool TrySerializeBinary(object value, Type valueType, string? propPath, out byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(valueType);

            Func<object, object>? serializer = null;
            if (!string.IsNullOrEmpty(propPath) && MemberSerializers.TryGetValue(propPath, out var memberSerializer))
            {
                if (!BinaryMemberSerializers.Contains(propPath))
                {
                    bytes = default!;
                    return false;
                }

                serializer = memberSerializer;
            }
            else
            {
                TypeSerializers.TryGetValue(valueType.TypeHandle, out serializer);
            }

            if (serializer is null || serializer(value) is not byte[] serialized)
            {
                bytes = default!;
                return false;
            }

            bytes = serialized;
            return true;
        }

        /// <summary>
        /// Invokes the effective registered binary serializer for one strongly typed runtime value without constructing an outer <see cref="InhetoBinary"/> envelope.<br/>
        /// This convenience preserves member-before-type resolution while allowing callers to retain compile-time value typing.<br/>
        /// The returned array is the handler's own result and is not cloned.<br/>
        /// </summary>
        /// <typeparam name="ValueType">The exact runtime value type used for type-wide handler resolution.<br/></typeparam>
        /// <param name="value">The non-null runtime value to convert.<br/></param>
        /// <param name="propPath">The optional Inheto property path used for member-specific handler precedence.<br/></param>
        /// <param name="bytes">Receives the handler-owned binary result when an effective binary serializer exists.<br/></param>
        /// <returns><c>true</c> when a binary handler was found and returned a non-null array; otherwise <c>false</c>.<br/></returns>
        public bool TrySerializeBinary<ValueType>(ValueType value, string? propPath, out byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(value);
            return TrySerializeBinary(value!, typeof(ValueType), propPath, out bytes);
        }

        internal SerializationOptions ForkForOperation()
        {
            var ret = new SerializationOptions
            {
                AllowMixedKeyTypesInDictionaryTypes = AllowMixedKeyTypesInDictionaryTypes,
                FailOnUnsupportedType = FailOnUnsupportedType,
                MemberTypes = MemberTypes
            };
            ret.IncludedMembers.UnionWith(IncludedMembers);
            ret.ExcludedMembers.UnionWith(ExcludedMembers);
            ret.ExcludedTypes.UnionWith(ExcludedTypes);
            ret.CircularProps.UnionWith(CircularProps);
            foreach (var entry in MemberSerializers)
                ret.MemberSerializers.Add(entry.Key, entry.Value);
            foreach (var entry in TypeSerializers)
                ret.TypeSerializers.Add(entry.Key, entry.Value);
            ret.BinaryMemberSerializers.UnionWith(BinaryMemberSerializers);
            return ret;
        }

    }
}
