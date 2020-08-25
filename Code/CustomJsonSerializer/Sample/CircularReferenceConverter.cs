using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using JsonSerializerApp.Entities;

namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Custom JSON converter that handles my type issues.
    /// </summary>
    /// <typeparam name="T">The type of converter.</typeparam>
    public partial class CircularReferenceConverter : CustomConverter<CircularReference>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularReferenceConverter{T}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public CircularReferenceConverter(CustomConverterOptions options)
            : base(options)
        {
        }

        #endregion

        #region Protected Methods | Overrides

        /// <inheritdoc/>
        protected override CustomJsonValueKind GetJsonValueKind(object value)
        {
            // To handle the serialization of a specified type

            if (value != null && value.GetType() == typeof(BadEnumerable))
            {
                // Redirect to the custom serialize

                return CustomJsonValueKind.Custom;
            }
            else
            {
                // For the other types follow the default behavior

                return base.GetJsonValueKind(value);
            }
        }

        /// <inheritdoc/>
        protected override void SerializeCustom<TValue>(Utf8JsonWriter writer, TValue value, CustomPropertyInfo property)
        {
            // To handle the serialization of a specified type

            if (value != null && value.GetType() == typeof(BadEnumerable))
            {
                // To suppress the serialization
                ////return;

                // To write a custom serialization
                writer.WritePropertyName(property.Name);
                writer.WriteStringValue("This type is an issue...");
            }
            else
            {
                // For the other types follow the default behavior

                base.SerializeCustom(writer, value, property);
            }
        }

        #endregion
    }
}
