namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Defines the serialization strategy to apply for the object type serialization.
    /// This option is useful to deserialize objects into the specified types.
    /// </summary>
    public enum TypeNameHandlingOption
    {
        /// <summary>
        /// Suppress the type.
        /// </summary>
        Suppress,

        /// <summary>
        /// Serialize the full name of the type.
        /// </summary>
        FullName,

        /// <summary>
        /// Serialize the short name of the type.
        /// </summary>
        ShortName,
    }
}
