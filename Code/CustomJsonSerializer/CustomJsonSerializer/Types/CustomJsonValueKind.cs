using System.Text.Json;

namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Defines an extended enumeration of the original <see cref="JsonValueKind"/> that contains more types
    /// to determine the best serialization strategy that should be applied.
    /// </summary>
    public enum CustomJsonValueKind : byte
    {
        /// <summary>
        /// There is no value (as distinct from <see cref="JsonValueKind.Null"/>).
        /// The undefined
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// A JSON object.
        /// </summary>
        Object = 1,

        /// <summary>
        /// A JSON array.
        /// </summary>
        Array = 2,

        /// <summary>
        /// A JSON string.
        /// </summary>
        String = 3,

        /// <summary>
        /// A JSON number.
        /// </summary>
        Number = 4,

        /// <summary>
        /// The JSON value true.
        /// </summary>
        True = 5,

        /// <summary>
        /// The JSON value false.
        /// </summary>
        False = 6,

        /// <summary>
        /// The JSON value null.
        /// </summary>
        Null = 7,

        /// <summary>
        /// Any value that requires a custom serialization.
        /// </summary>
        Custom = 99
    }
}
