namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Defines the serialization strategy to apply when a circular reference is detected.
    /// A circular serialization occurs when an object contains another object that was serialized before.
    /// The hash code of the object is used to identify a circular reference.
    /// </summary>
    public enum CircularRefHandlingOption
    {
        /// <summary>
        /// Suppress the value.
        /// </summary>
        Suppress,

        /// <summary>
        /// Serialize the value.
        /// </summary>
        Value,

        /// <summary>
        /// Serialize the hash code of the value.
        /// </summary>
        HashCode
    }
}
