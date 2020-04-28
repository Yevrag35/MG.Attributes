using System;
using System.Collections.Generic;

namespace MG.Attributes
{
    /// <summary>
    /// An interface exposing methods for retrieving secondary object values from 
    /// <see cref="Enum"/> value types.
    /// </summary>
    public interface IValueEnumGetter
    {
        /// <summary>
        /// Retrieves a secondary value from an <see cref="IValueAttribute"/> attribute attached to 
        /// an <see cref="Enum"/> value type.
        /// </summary>
        /// <typeparam name="T1">The type of the retrieved value.</typeparam>
        /// <typeparam name="T2">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="enumValue">The specific enumeration value that has the attached <see cref="IValueAttribute"/> attribute.</param>
        T1 GetAttributeValue<T1, T2>(Enum enumValue)
            where T2 : Attribute, IValueAttribute;

        /// <summary>
        /// Retrieves the secondary values from one or multiple <see cref="IValueAttribute"/> attributes attached
        /// to a single <see cref="Enum"/> value type.
        /// </summary>
        /// <typeparam name="T1">The type of the retrieved value.</typeparam>
        /// <typeparam name="T2">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="enumValue">The specific enumeration value that has the attached <see cref="IValueAttribute"/> attributes.</param>
        IEnumerable<T1> GetAttributeValues<T1, T2>(Enum enumValue)
            where T2 : Attribute, IValueAttribute;
    }
}
