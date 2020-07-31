using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonSerializerApp.Serialization
{
    /// <inheritdoc/>
    public partial class CustomConverter<T> : JsonConverter<T>
    {
        #region Protected Methods

        /// <summary>
        /// Gets the JSON value kind of the specified object which determines the method to serialize it.
        /// </summary>
        /// <param name="value">The object value.</param>
        /// <returns>The JSON value kind.</returns>
        protected virtual CustomJsonValueKind GetJsonValueKind(object value)
        {
            // Null

            if (value == null)
            {
                return CustomJsonValueKind.Null;
            }

            var type = value.GetType();

            // Array

            if (type.IsArray)
            {
                return CustomJsonValueKind.Array;
            }

            // Dictionary

            if (type.IsIDictionary())
            {
                return CustomJsonValueKind.Object;
            }

            // Enumerable

            if (type.IsIEnumerable())
            {
                return CustomJsonValueKind.Array;
            }

            // Number

            if (type.IsNumber())
            {
                return CustomJsonValueKind.Number;
            }

            // Boolean

            if (type == typeof(bool))
            {
                if ((bool)value)
                {
                    return CustomJsonValueKind.True;
                }
                else
                {
                    return CustomJsonValueKind.False;
                }
            }

            // Other Primitives

            if (type == typeof(string)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid)
                || type.IsPrimitive)
            {
                return CustomJsonValueKind.String;
            }

            // Object

            if ((type.GetProperties()?.Length ?? 0) > 0)
            {
                return CustomJsonValueKind.Object;
            }

            // Unknown

            return CustomJsonValueKind.Undefined;
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        protected virtual void Serialize<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options)
        {
            this.Serialize(writer, value, options, new List<int>());
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="hashCodes">The reference hash codes.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void Serialize<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, List<int> hashCodes, CustomPropertyInfo property = null)
        {
            // Get the value kind

            var jsonValueType = this.GetJsonValueKind(value);

            // Maximum depth detection
            // with a default value of 0 indicating a maximum depth of 64

            if (jsonValueType == CustomJsonValueKind.Object
                || jsonValueType == CustomJsonValueKind.Array)
            {
                int maxDepth = options.MaxDepth > 0 ? options.MaxDepth : 64;

                if (writer.CurrentDepth > maxDepth)
                {
                    if (this.SerializeMaxDepth(writer, value, options, hashCodes, property))
                    {
                        return;
                    }
                }
            }

            // Circular reference detection

            if (jsonValueType == CustomJsonValueKind.Object
                || jsonValueType == CustomJsonValueKind.Array)
            {
                var hashCode = value.GetHashCode();

                if (hashCodes.Contains(hashCode))
                {
                    if (this.SerializeCircularRef(writer, value, options, hashCodes, property))
                    {
                        return;
                    }
                }
                else
                {
                    hashCodes.Add(hashCode);
                }
            }

            // Write an Enumeration

            if (value != null && value.GetType().IsEnum)
            {
                this.SerializeEnumeration(writer, value, options, property);
                return;
            }

            // Write other JSON value types

            switch (jsonValueType)
            {
                case CustomJsonValueKind.Null:
                    this.SerializeNull(writer, value, options, property);
                    break;

                case CustomJsonValueKind.True:
                    this.SerializeBool(writer, true, options, property);
                    break;

                case CustomJsonValueKind.False:
                    this.SerializeBool(writer, false, options, property);
                    break;

                case CustomJsonValueKind.String:
                    this.SerializeString(writer, value, options, property);
                    break;

                case CustomJsonValueKind.Number:
                    this.SerializeNumber(writer, value, options, property);
                    break;

                case CustomJsonValueKind.Array:
                    this.SerializeArray(writer, value, options, hashCodes, property);
                    break;

                case CustomJsonValueKind.Object:
                    this.SerializeObject(writer, value, options, hashCodes, property);
                    break;

                case CustomJsonValueKind.Undefined:
                    this.SerializeUndefined(writer, value, options, hashCodes, property);
                    break;

                case CustomJsonValueKind.Custom:
                    this.SerializeCustom(writer, value, options, hashCodes, property);
                    break;

                default:
                    return;
            }
        }

        /// <summary>
        /// Serializes the specified value when a circular reference is detected.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="hashCodes">The reference hash codes.</param>
        /// <param name="property">The property metadata.</param>
        /// <returns><c>True</c> if the serialization was handled; otherwise <c>false</c> to continue with default serialization.</returns>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual bool SerializeCircularRef<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, List<int> hashCodes, CustomPropertyInfo property = null)
        {
            switch (options.CircularRefHandling)
            {
                case CircularRefHandlingOption.Suppress:
                    return true;

                case CircularRefHandlingOption.Value:
                    return false;

                case CircularRefHandlingOption.HashCode:

                    if (property != null)
                    {
                        property.Name = $"{property.Name}$HashCodeRef";
                        this.SerializePropertyName(writer, value, options, property);
                        writer.WriteStringValue($"{value.GetHashCode()}");
                    }

                    return true;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Serializes the specified value when maximum depth is achieved.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="hashCodes">The reference hash codes.</param>
        /// <param name="property">The property metadata.</param>
        /// <returns><c>True</c> if the serialization was handled; otherwise <c>false</c> to continue with default serialization.</returns>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual bool SerializeMaxDepth<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, List<int> hashCodes, CustomPropertyInfo property = null)
        {
            switch (this.Options.MaxDepthHandling)
            {
                case MaxDepthHandlingOption.Suppress:
                    return true;

                case MaxDepthHandlingOption.Value:
                    return false;

                case MaxDepthHandlingOption.CurrentDepth:

                    if (property != null)
                    {
                        property.Name = $"{property.Name}$MaxDepth";
                        this.SerializePropertyName(writer, value, options, property);
                        writer.WriteStringValue($"{writer.CurrentDepth}");
                    }

                    return true;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Serializes the type name of specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeTypeName<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, CustomPropertyInfo property = null)
        {
            if (value != null)
            {
                switch (options.TypeNameHandling)
                {
                    case TypeNameHandlingOption.Suppress:
                        break;

                    case TypeNameHandlingOption.ShortName:
                        writer.WritePropertyName($"$Type");
                        writer.WriteStringValue(JsonEncodedText.Encode(value.GetType().ShortAssemblyQualifiedName()));
                        break;

                    case TypeNameHandlingOption.FullName:
                        writer.WritePropertyName($"$Type");
                        writer.WriteStringValue(JsonEncodedText.Encode(value.GetType().FullName));
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Serializes the specified number.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializePropertyName<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, CustomPropertyInfo property = null)
        {
            if (property != null && !property.Name.IsNullOrEmpty())
            {
                writer.WritePropertyName(property.Name);
            }
        }

        /// <summary>
        /// Serializes the hash code of the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeHashCode<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options)
        {
            if (value != null && options.CircularRefHandling == CircularRefHandlingOption.HashCode)
            {
                writer.WritePropertyName($"$HashCode");
                writer.WriteStringValue($"{value.GetHashCode()}");
            }
        }

        /// <summary>
        /// Serializes the null value.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeNull<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, CustomPropertyInfo property = null)
        {
            if (property != null && !property.IsContainer)
            {
                this.SerializePropertyName(writer, value, options, property);
                writer.WriteNullValue();
            }
        }

        /// <summary>
        /// Serializes the specified boolean.
        /// </summary>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeBool(Utf8JsonWriter writer, bool value, CustomConverterOptions options, CustomPropertyInfo property = null)
        {
            this.SerializePropertyName(writer, value, options, property);
            writer.WriteBooleanValue(value);
        }

        /// <summary>
        /// Serializes the specified enumeration.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeEnumeration<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, CustomPropertyInfo property = null)
        {
            this.SerializePropertyName(writer, value, options, property);
            writer.WriteStringValue(Enum.GetName(value.GetType(), value));
        }

        /// <summary>
        /// Serializes the specified number.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeNumber<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, CustomPropertyInfo property = null)
        {
            this.SerializePropertyName(writer, value, options, property);
            var num = Convert.ToDecimal(value, CultureInfo.CurrentCulture);
            writer.WriteNumberValue(num);
        }

        /// <summary>
        /// Serializes the specified string.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeString<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, CustomPropertyInfo property = null)
        {
            this.SerializePropertyName(writer, value, options, property);
            var result = JsonSerializer.Serialize(value).Replace("\u0022", string.Empty, StringComparison.OrdinalIgnoreCase);
            writer.WriteStringValue(result);
        }

        /// <summary>
        /// Serializes the specified enumerable.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="hashCodes">The reference hash codes.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeEnumerable<TValue>(Utf8JsonWriter writer, IEnumerable<TValue> value, CustomConverterOptions options, List<int> hashCodes, CustomPropertyInfo property = null)
        {
            this.SerializePropertyName(writer, value, options, property);

            foreach (var item in value)
            {
                this.Serialize(writer, item, options, hashCodes);
            }
        }

        /// <summary>
        /// Serializes the specified undefined type.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="hashCodes">The reference hash codes.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeUndefined<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, List<int> hashCodes, CustomPropertyInfo property = null)
        {
            this.SerializePropertyName(writer, value, options, property);
            writer.WriteStringValue($"$Undefined");
        }

        /// <summary>
        /// Serializes the specified custom type.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="hashCodes">The reference hash codes.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeCustom<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, List<int> hashCodes, CustomPropertyInfo property = null)
        {
            return;
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="hashCodes">The reference hash codes.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeObject<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, List<int> hashCodes, CustomPropertyInfo property = null)
        {
            this.SerializePropertyName(writer, value, options, property);

            writer.WriteStartObject();

            this.SerializeTypeName(writer, value, options, property);
            SerializeHashCode(writer, value, options);

            var type = value.GetType();

            if (type.IsIDictionary())
            {
                var dict = value as IDictionary;

                foreach (var key in dict.Keys)
                {
                    var dictProperty = new CustomPropertyInfo(key.ToString());
                    this.Serialize(writer, dict[key], options, hashCodes, dictProperty);
                }
            }
            else
            {
                foreach (var prop in type.GetProperties().Where(t => t.DeclaringType.FullName != "System.Linq.Dynamic.Core.DynamicClass"))
                {
                    var objProperty = new CustomPropertyInfo(prop);
                    this.Serialize(writer, prop.GetValue(value), options, hashCodes, objProperty);
                }
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serializes the specified array.
        /// </summary>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="writer">The serializer writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        /// <param name="hashCodes">The reference hash codes.</param>
        /// <param name="property">The property metadata.</param>
        [SuppressMessage("Build", "CA1031:Modify 'IsNullOrEmpty' to catch a more specific allowed exception type, or rethrow the exception.", Justification = "Required to implement the specified behavior.")]
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Validation is not required because null arguments are expected or not null values are granted by the converter implementation.")]
        protected virtual void SerializeArray<TValue>(Utf8JsonWriter writer, TValue value, CustomConverterOptions options, List<int> hashCodes, CustomPropertyInfo property = null)
        {
            this.SerializePropertyName(writer, value, options, property);
            writer.WriteStartArray();

            try
            {
                List<object> list = new List<object>();

                if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
                {
                    IEnumerable items = (IEnumerable)value;

                    // Money type: the GetEnumerator() returns null...
                    if (!items.IsNullOrEmpty())
                    {
                        foreach (var item in items)
                        {
                            list.Add(item);
                        }
                    }
                }
                else if (value is IEnumerable<object>)
                {
                    foreach (var item in value as IEnumerable<object>)
                    {
                        list.Add(item);
                    }
                }
                else if (value is IOrderedEnumerable<object>)
                {
                    foreach (var item in value as IOrderedEnumerable<object>)
                    {
                        list.Add(item);
                    }
                }

                this.SerializeEnumerable(writer, list, options, hashCodes, null);
            }
            catch
            {
                // Upon failure, use reflection and generic SerializeEnumerable method

                Type[] args = value.GetType().GetGenericArguments();
                Type itemType = args[0];

                MethodInfo serializeEnumerableMethod = typeof(CustomConverter<T>)
                    .GetMethod("SerializeEnumerable", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(itemType);

                serializeEnumerableMethod.Invoke(null, new object[] { writer, value, options, hashCodes, property });
            }

            writer.WriteEndArray();
        }
    }

    #endregion
}