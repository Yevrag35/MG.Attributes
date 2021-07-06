using System;
using System.Collections.Generic;

namespace MG.Attributes
{
    public interface IValueRecaller
    {
        /// <summary>
        /// Gets the first value from the attributed <see cref="IValueAttribute"/> of the enumeration value and casts 
        /// it to the type <typeparamref name="TOutput"/>.
        /// </summary>
        /// <typeparam name="TOutput">The type of the underlying value.</typeparam>
        /// <typeparam name="TAttribute">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        TOutput GetAttributeValue<TOutput, TAttribute>(Enum enumValue) where TAttribute : Attribute, IValueAttribute;

        /// <summary>
        /// Gets the first value from the attributed <see cref="IValueAttribute"/> of the <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        object GetAttributeValue<TAttribute>(Enum enumValue) where TAttribute : Attribute, IValueAttribute;

        TOutput[] GetAttributeValues<TAttribute, TOutput>(Enum enumValue)
            where TAttribute : Attribute, IAttributeValueCollection;
    }
}
