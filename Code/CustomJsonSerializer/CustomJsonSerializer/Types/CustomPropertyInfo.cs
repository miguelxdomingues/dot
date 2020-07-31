using System;
using System.Reflection;

namespace JsonSerializerApp.Serialization
{
    /// <summary>
    /// Provides a pseudo property that encapsulates an underlying property information with an extended behavior.
    /// The pseudo property is created at run time using the original property information or providing the required property metadata.
    /// </summary>
    /// <seealso cref="PropertyInfo" />
    public class CustomPropertyInfo
    {
        #region Fields

        private string pseudoName = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropertyInfo" /> class.
        /// </summary>
        /// <param name="propertyInfo">The underlying property information.</param>
        public CustomPropertyInfo(PropertyInfo propertyInfo)
        {
            this.BasePropertyInfo = propertyInfo ?? throw new ArgumentException(nameof(propertyInfo));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropertyInfo" /> class.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        public CustomPropertyInfo(string propertyName)
        {
            this.pseudoName = propertyName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the underlying property information.
        /// </summary>
        public PropertyInfo BasePropertyInfo { get; }

        /// <summary>
        /// Gets the name of the current member.
        /// </summary>
        public string Name
        {
            get
            {
                return this.pseudoName ?? this.BasePropertyInfo?.Name;
            }

            set
            {
                this.pseudoName = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the property type is a container.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the property type is a container; otherwise, <c>false</c>.
        /// </value>
        public bool IsContainer
        {
            get
            {
                return this.BasePropertyInfo != null ? this.BasePropertyInfo.PropertyType.IsContainer() : false;
            }
        }

        #endregion
    }
}
