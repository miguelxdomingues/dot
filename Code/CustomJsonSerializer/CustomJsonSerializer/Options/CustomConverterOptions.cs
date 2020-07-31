using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Provides options to be used with <see cref="CustomConverter{T}"/>.
    /// </summary>
    public sealed class CustomConverterOptions
    {
        #region Public Properties | JsonSerializerOptions

        /// <summary>
        /// Gets or sets the JSON serializer options supported by the underlying <see cref="JsonSerializer"/>.
        /// </summary>
        /// <value>
        /// The JSON serializer options.
        /// </value>
        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        /// <summary>
        /// Get or sets a value that indicates whether an extra comma at the end of a list
        /// of JSON values in an object or array is allowed (and ignored) within the JSON
        /// payload being deserialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if an extra comma at the end of a list of JSON values in an object or array is allowed (and ignored); <c>false</c> otherwise.
        /// </value>
        /// <exception cref="System.InvalidOperationException">This property was set after serialization or deserialization has occurred.</exception>
        public bool AllowTrailingCommas
        {
            get
            {
                return this.JsonSerializerOptions.AllowTrailingCommas;
            }
            set
            {
                JsonSerializerOptions.AllowTrailingCommas = value;
            }
        }

        /// <summary>
        /// Gets or sets the default buffer size, in bytes, to use when creating temporary buffers.
        /// </summary>
        /// <value>
        ///     The default buffer size in bytes.
        /// </value>
        /// <exception cref="System.ArgumentException">The buffer size is less than 1.</exception>
        /// <exception cref="System.InvalidOperationException">This property was set after serialization or deserialization has occurred.</exception>
        public int DefaultBufferSize
        {
            get
            {
                return this.JsonSerializerOptions.DefaultBufferSize;
            }
            set
            {
                JsonSerializerOptions.DefaultBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the encoder to use when escaping strings, or null to use the default encoder.
        /// </summary>
        /// <value>
        ///     The JavaScript character encoding.
        /// </value>
        public JavaScriptEncoder Encoder
        {
            get
            {
                return this.JsonSerializerOptions.Encoder;
            }
            set
            {
                JsonSerializerOptions.Encoder = value;
            }
        }

        /// <summary>
        /// Gets or sets the policy used to convert a System.Collections.IDictionary key's
        /// name to another format, such as camel-casing.
        /// </summary>
        /// <value>
        ///     The policy used to convert a System.Collections.IDictionary key's name to another format.
        /// </value>
        public JsonNamingPolicy DictionaryKeyPolicy
        {
            get
            {
                return this.JsonSerializerOptions.DictionaryKeyPolicy;
            }
            set
            {
                JsonSerializerOptions.DictionaryKeyPolicy = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether null values are ignored during serialization
        /// and deserialization. The default value is false.
        /// </summary>
        /// <value>
        ///     <c>true</c> to ignore null values during serialization and deserialization; <c>false</c> otherwise.
        /// </value>
        /// <exception cref="System.InvalidOperationException">This property was set after serialization or deserialization has occurred.</exception>
        public bool IgnoreNullValues
        {
            get
            {
                return this.JsonSerializerOptions.IgnoreNullValues;
            }
            set
            {
                JsonSerializerOptions.IgnoreNullValues = value;
            }
        }

        /// <summary>
        /// Gets a value that determines whether read-only properties are ignored during
        /// serialization. The default value is false.
        /// </summary>
        /// <value>
        ///     <c>true</c> to ignore read-only properties during serialization; <c>false</c> otherwise.
        /// </value>
        /// <exception cref="System.InvalidOperationException">This property was set after serialization or deserialization has occurred.</exception>
        public bool IgnoreReadOnlyProperties
        {
            get
            {
                return this.JsonSerializerOptions.IgnoreReadOnlyProperties;
            }
            set
            {
                JsonSerializerOptions.IgnoreReadOnlyProperties = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum depth allowed when serializing or deserializing JSON,
        /// with the default value of 0 indicating a maximum depth of 64.
        /// </summary>
        /// <value>
        ///     The maximum depth allowed when serializing or deserializing JSON.
        /// </value>
        /// <exception cref="System.ArgumentException">The max depth is set to a negative value.</exception>
        /// <exception cref="System.InvalidOperationException">This property was set after serialization or deserialization has occurred.</exception>
        public int MaxDepth
        {
            get
            {
                return this.JsonSerializerOptions.MaxDepth;
            }
            set
            {
                JsonSerializerOptions.MaxDepth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the policy used to convert a property's name
        /// on an object to another format, such as camel-casing, or null to leave property
        /// names unchanged.
        /// </summary>
        /// <value>
        ///     A property naming policy, or null to leave property names unchanged.
        /// </value>
        public JsonNamingPolicy PropertyNamingPolicy
        {
            get
            {
                return this.JsonSerializerOptions.PropertyNamingPolicy;
            }
            set
            {
                JsonSerializerOptions.PropertyNamingPolicy = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether a property's name uses a case-insensitive
        /// comparison during deserialization. The default value is false.
        /// </summary>
        /// <value>
        ///     <c>true</c> to compare property names using case-insensitive comparison; <c>false</c> otherwise.
        /// </value>
        public bool PropertyNameCaseInsensitive
        {
            get
            {
                return this.JsonSerializerOptions.PropertyNameCaseInsensitive;
            }
            set
            {
                JsonSerializerOptions.PropertyNameCaseInsensitive = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that defines how comments are handled during deserialization.
        /// </summary>
        /// <value>
        ///     A value that indicates whether comments are allowed, disallowed, or skipped.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">The comment handling enumeration is set to a value that is not supported (or not within the <see cref="System.Text.Json.JsonCommentHandling"/> enumeration range).</exception>
        /// <exception cref="System.InvalidOperationException">This property was set after serialization or deserialization has occurred.</exception>
        public JsonCommentHandling ReadCommentHandling
        {
            get
            {
                return this.JsonSerializerOptions.ReadCommentHandling;
            }
            set
            {
                JsonSerializerOptions.ReadCommentHandling = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that defines whether JSON should use pretty printing.
        /// By default, JSON is serialized without any extra white space.
        /// </summary>
        /// <value>
        ///     <c>true</c> if JSON should pretty print on serialization; <c>false</c> otherwise (default).
        /// </value>
        /// <exception cref="System.InvalidOperationException">This property was set after serialization or deserialization has occurred.</exception>
        public bool WriteIndented
        {
            get
            {
                return this.JsonSerializerOptions.WriteIndented;
            }
            set
            {
                JsonSerializerOptions.WriteIndented = value;
            }
        }

        /// <summary>
        /// Gets the list of user-defined converters that were registered.
        /// </summary>
        /// <value>
        ///     The list of custom converters.
        /// </value>
        /// <exception cref="">
        /// </exception>
        public IList<JsonConverter> Converters
        {
            get
            {
                return this.JsonSerializerOptions.Converters;
            }
        }

        #endregion

        #region Public Properties | Extended Properties

        /// <summary>
        /// Gets or sets the circular reference handling option that defines
        /// the serialization strategy to apply when a circular reference is detected.
        /// </summary>
        /// <value>
        ///     The circular reference handling option.
        /// </value>
        public CircularRefHandlingOption CircularRefHandling { get; set; }

        /// <summary>
        /// Gets or sets the maximum depth handling option that defines
        /// the serialization strategy to apply when the maximum depth is achieved.
        /// </summary>
        /// <value>
        ///     The circular reference handling option.
        /// </value>
        public MaxDepthHandlingOption MaxDepthHandling { get; set; }

        /// <summary>
        /// Gets or sets type name handling option that defines
        /// the serialization strategy to apply for the object type serialization.
        /// </summary>
        /// <value>
        ///   the type name handling option.
        /// </value>
        public TypeNameHandlingOption TypeNameHandling { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomConverterOptions"/> class.
        /// </summary>
        public CustomConverterOptions()
        {
            // Initialize with default values

            this.JsonSerializerOptions = DefaultSerializerOptions();

            this.MaxDepthHandling = MaxDepthHandlingOption.Suppress;
            this.CircularRefHandling = CircularRefHandlingOption.Suppress;
            this.TypeNameHandling = TypeNameHandlingOption.Suppress;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializerOptions"/> with default values.
        /// </summary>
        /// <returns>The options object.</returns>
        public static JsonSerializerOptions DefaultSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                IgnoreNullValues = false,
                IgnoreReadOnlyProperties = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = null, // Preserve original keys
                PropertyNameCaseInsensitive = true, // Web API compliant
                WriteIndented = false,
                MaxDepth = 0, // Max recursive depth
            };
        }

        #endregion

        #region Public Methods | JsonSerializerOptions

        /// <summary>
        /// Gets the converter for the specified type.
        /// </summary>
        /// <param name="typeToConvert">The type to return the converter for.</param>
        /// <returns>
        /// The first converter that supports the given type, or null if there is no converter.
        /// </returns>
        public JsonConverter GetConverter(Type typeToConvert)
        {
            return this.JsonSerializerOptions.GetConverter(typeToConvert);
        }

        #endregion
    }
}