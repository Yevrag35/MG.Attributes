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
        /// <typeparam name="TOutput">The type of the retrieved value.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="enumValue">The specific enumeration value that has the attached <see cref="IValueAttribute"/> attribute.</param>
        TOutput GetAttributeValue<TAtt, TOutput>(Enum enumValue)
            where TAtt : Attribute, IValueAttribute;

        /// <summary>
        /// Retrieves the secondary values from one or multiple <see cref="IValueAttribute"/> attributes attached
        /// to a single <see cref="Enum"/> value type.
        /// </summary>
        /// <typeparam name="TOutput">The type of the retrieved value.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="enumValue">The specific enumeration value that has the attached <see cref="IValueAttribute"/> attributes.</param>
        IEnumerable<TOutput> GetAttributeValues<TAtt, TOutput>(Enum enumValue)
            where TAtt : Attribute, IValueAttribute;

        /// <summary>
        /// Attempts to retrieve the secondary value from an <see cref="IValueAttribute"/> attribute attached to a
        /// single <see cref="Enum"/> value type returning the success or failure of that operation.
        /// </summary>
        /// <typeparam name="TOutput">The type of the retrieved value.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="enumValue">The specific enumeration value that has the attached <see cref="IValueAttribute"/> attributes.</param>
        /// <param name="outValue">The casted value retrieved from the attribute.</param>
        bool TryGetAttributeValue<TAtt, TOutput>(Enum enumValue, out TOutput outValue, out Exception caughtException)
            where TAtt : Attribute, IValueAttribute;

        /// <summary>
        /// Get the enum value that has an <see cref="IValueAttribute"/> attribute with the matching value.
        /// </summary>
        ///// <typeparam name="T1">The type of the value that is supplied.</typeparam>
        /// <typeparam name="TOutput">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValue">The object that <see cref="IValueAttribute.Value"/> should equal.</param>
        TOutput GetEnumFromValue<TOutput, TAtt>(object objValue)
            where TOutput : Enum
            where TAtt : Attribute, IValueAttribute;

        /// <summary>
        /// Get the enum values that have attached an <see cref="IValueAttribute"/> attribute which contains any overlapping 
        /// items of the specified collection.
        /// </summary>
        /// <typeparam name="TInput">The type of the collection's items.</typeparam>
        /// <typeparam name="TOutput">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValues">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        IEnumerable<TOutput> GetEnumsFromValues<TOutput, TAtt, TInput>(IEnumerable<TInput> objValues)
            where TOutput : Enum
            where TAtt : Attribute, IValueAttribute;

        /// <summary>
        /// Get the enum value that has an <see cref="IValueAttribute"/> attribute with the matching collection.
        /// of values.
        /// </summary>
        /// <typeparam name="TInput">The type of the collection's items.</typeparam>
        /// <typeparam name="TOutput">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValues">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        TOutput GetEnumFromValues<TOutput, TAtt, TInput>(IEnumerable<TInput> objValues)
            where TOutput : Enum
            where TAtt : Attribute, IValueAttribute;
    }
}
