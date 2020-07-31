using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Provides the <see cref="Type"/> and <see cref="object"/> extensions required by the <see cref="CustomConverter{T}"/> implementation.
    /// </summary>
    internal static class CustomConverterExtensions
    {
        #region Internal Methods

        /// <summary>
        /// Gets the type of the business entity.
        /// This method deals with the Entity Framework dynamic proxies and returns
        /// the underlying type (the original type of the business entity).</summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="value">The object value.</param>
        /// <returns>The type of the business entity.</returns>
        internal static Type GetEntityType<T>(this T value)
        {
            return (value as object).GetEntityType();
        }

        /// <summary>
        /// Gets the type of the object.
        /// Handles the Entity Framework dynamic proxies and returns
        /// the underlying type (the original type of the entity).
        /// </summary>
        /// <param name="instance">The entity.</param>
        /// <returns>The type of the business entity.</returns>
        internal static Type GetEntityType(this object instance)
        {
            // Validation

            if (instance == null)
            {
                return null;
            }

            // Default result

            Type entityType = instance.GetType();

            // Is it a dynamic proxy?

            if (!string.IsNullOrEmpty(entityType.Namespace)
                && entityType.Namespace.Equals("System.Data.Entity.DynamicProxies", StringComparison.OrdinalIgnoreCase))
            {
                return entityType.BaseType;
            }

            // Result

            return entityType;
        }

        /// <summary>Determines whether [is i enumerable].</summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is i enumerable] [the specified type]; otherwise, <c>false</c>.</returns>
        internal static bool IsIEnumerable(this Type type)
        {
            return type != typeof(string)
                && type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        /// <summary>Determines whether [is i dictionary].</summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is i dictionary] [the specified type]; otherwise, <c>false</c>.</returns>
        internal static bool IsIDictionary(this Type type)
        {
            return
                type.GetInterfaces().Contains(typeof(IDictionary))
                || (type.IsGenericType && typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }

        /// <summary>Determines whether this instance is number.</summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is number; otherwise, <c>false</c>.</returns>
        internal static bool IsNumber(this Type type)
        {
            return type == typeof(byte)
                || type == typeof(ushort)
                || type == typeof(short)
                || type == typeof(uint)
                || type == typeof(int)
                || type == typeof(ulong)
                || type == typeof(long)
                || type == typeof(decimal)
                || type == typeof(double)
                || type == typeof(float);
        }

        /// <summary>
        /// Determines whether the specified type is a container.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is a container; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsContainer(this Type type)
        {
            if (type.IsArray
             || type.IsIDictionary()
             || type.IsIEnumerable())
            {
                return true;
            }
            else if (type.IsNumber()
                  || type == typeof(bool)
                  || type == typeof(string)
                  || type == typeof(DateTime)
                  || type == typeof(DateTimeOffset)
                  || type == typeof(TimeSpan)
                  || type.IsPrimitive)
            {
                return false;
            }
            else if ((type.GetProperties()?.Length ?? 0) > 0)
            {
                return true;
            }
            else if (type == typeof(object))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is null or empty].
        /// </summary>
        /// <typeparam name="T">The type of enumerable.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified items]; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Build", "CA1031:Modify 'IsNullOrEmpty' to catch a more specific allowed exception type, or rethrow the exception.", Justification = "Required to implement the specified behavior.")]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            try
            {
                // Null or empty collection
                return items == null || !items.Any();
            }
            catch (Exception)
            {
                // GetEnumerator is not implemented
                return true;
            }
        }

        /// <summary>
        /// Determines whether [is null or empty].
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified items]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this IEnumerable items)
        {
            return items.Cast<object>().IsNullOrEmpty();
        }

        /// <summary>
        /// Gets the assembly qualified name of the specified type stripping off the
        /// version, culture and public key token parts.
        /// </summary>
        /// <param name="value">The type.</param>
        /// <returns>The short assembly qualified name.</returns>
        public static string ShortAssemblyQualifiedName(this Type value)
        {
            if (value != null)
            {
                var name = value.AssemblyQualifiedName;

                name = Regex.Replace(name, @", Version=\d+.\d+.\d+.\d+", string.Empty);
                name = Regex.Replace(name, @", Culture=\w+", string.Empty);
                name = Regex.Replace(name, @", PublicKeyToken=\w+", string.Empty);

                return name;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
