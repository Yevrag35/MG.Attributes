using System;
using System.Collections.Generic;

namespace MG.Attributes
{
    /// <summary>
    /// An interface exposing the methods for reading values from <see cref="IValueAttribute"/>.
    /// </summary>
    public interface IAttributeValueReader
    {
        /// <summary>
        /// Gets the first value from the attributed <see cref="IValueAttribute"/> of the enumeration value and casts 
        /// it to the type <typeparamref name="TOutput"/>.
        /// </summary>
        /// <typeparam name="TOutput">The type of the underlying value.</typeparam>
        /// <typeparam name="TAttribute">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <returns>
        ///     The value specified in <see cref="IValueAttribute.Value"/> cast as <typeparamref name="TOutput"/>.
        /// </returns>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        TOutput GetAttributeValue<TOutput, TAttribute>(Enum enumValue) where TAttribute : Attribute, IValueAttribute;

        /// <summary>
        /// Gets the first value from the attributed <see cref="IValueAttribute"/> of the <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <returns>
        ///     The <see cref="object"/> value specified in <see cref="IValueAttribute.Value"/>.
        /// </returns>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        object GetAttributeValue<TAttribute>(Enum enumValue) where TAttribute : Attribute, IValueAttribute;

        /// <summary>
        /// Gets the first value from the attributed <see cref="IValueAttribute"/> of the enumeration value and casts 
        /// it to the type <typeparamref name="TOutput"/>.
        /// </summary>
        /// <typeparam name="TOutput">The type of the underlying value.</typeparam>
        /// <typeparam name="TAttribute">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <returns>
        ///     A single-dimension array of values as <typeparamref name="TOutput"/>.
        /// </returns>
        TOutput[] GetAttributeValues<TAttribute, TOutput>(Enum enumValue)
            where TAttribute : Attribute, IAttributeValueCollection;
    }
}
