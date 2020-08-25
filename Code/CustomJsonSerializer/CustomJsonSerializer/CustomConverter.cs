using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Provides a custom JSON converter that supports the following features:
    /// polymorphic serialization for type inheritance, maximum depth for larger objects,
    /// reference loop handling for circular references, dynamic type handling
    /// for navigation properties like the Entity Framework,
    /// and pattern matching for exclusion types.
    /// </summary>
    /// <typeparam name="T">The type of converter.</typeparam>
    public partial class CustomConverter<T> : JsonConverter<T>
    {
        #region Protected Properties

        /// <summary>
        /// Gets or sets the custom converter options that defines
        /// the common <see cref="JsonSerializerOptions"/> used by the <see cref="JsonSerializer"/>
        /// and extended options used by the <see cref="CustomConverter{T}"/>.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        protected CustomConverterOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the list of hash codes to detect circular references.
        /// </summary>
        /// <value>
        /// The list of hash codes.
        /// </value>
        protected List<int> HashCodes { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomConverter{T}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public CustomConverter(CustomConverterOptions options)
        {
            this.Options = options;
        }

        #endregion

        #region Public Methods | JsonConverter Overrides

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return base.CanConvert(typeToConvert);
        }

        /// <inheritdoc />
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Use default deserialize
            return (T)JsonSerializer.Deserialize<T>(ref reader, options);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // Update JSON serializer options (if any has changed)
            this.Options.JsonSerializerOptions = options;

            // Initialize the list of hash codes to detect circular references
            this.HashCodes = new List<int>();

            // Use custom serialize with custom options
            this.Serialize(writer, value);
        }

        #endregion
    }
}