namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Defines the serialization strategy to apply when the maximum depth is achieved.
    /// Maximum depth is the number of recursive levels the serialization can handle.
    /// This option is useful to serialize large objects.
    /// </summary>
    public enum MaxDepthHandlingOption
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
        /// Serialize the current depth.
        /// </summary>
        CurrentDepth
    }
}
