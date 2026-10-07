






namespace Inheto
{
    //====== TYPES ======
    /// <summary>
    /// Selects which instance properties and fields participate in serialization or materialization; visibility selection does not bypass getter or setter behavior.<br/>
    /// </summary>
    public enum MemberTypesEnum
    {
        /// <summary>
        /// Include public instance properties and fields.<br/>
        /// </summary>
        PublicPropertiesAndFields,
        /// <summary>
        /// Include public instance properties, not fields.<br/>
        /// </summary>
        PublicPropertiesOnly,
        /// <summary>
        /// Include public instance fields, not properties.<br/>
        /// </summary>
        PublicFieldsOnly,
        /// <summary>
        /// Include instance fields at public and non-public visibility, not properties.<br/>
        /// </summary>
        PublicAndPrivateFields,
        /// <summary>
        /// Include non-public instance fields, not properties.<br/>
        /// </summary>
        PrivateFieldsOnly
    }
}
