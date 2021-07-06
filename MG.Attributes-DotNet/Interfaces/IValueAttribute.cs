using System;
using System.ComponentModel;

namespace MG.Attributes
{
    /// <summary>
    /// An interface exposing a value property and
    /// method for getting an underlying value with a cast.  Used on <see cref="Attribute"/> implementations.
    /// </summary>
    public interface IValueAttribute
    {
        /// <summary>
        /// The specified value given during the <see cref="IValueAttribute"/> implementing class's construction.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Returns the held value of the <see cref="IValueAttribute"/> and casts it to the 
        /// specified type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        ///     Dynamics are not supported.
        /// </remarks>
        /// <typeparam name="T">
        ///     The type to cast <see cref="Value"/>'s 
        ///     property as.
        /// </typeparam>
        /// <exception cref="InvalidCastException">Thrown when the object value
        /// can't be explicitly cast as the given type.  
        /// </exception>
        T GetAs<T>();

        /// <summary>
        /// Returns a <see cref="bool"/> value specifying if <see cref="IValueAttribute.Value"/>
        /// is a <see cref="string"/>.
        /// </summary>
        /// <returns>'True' is the value is a <see cref="string"/>; otherwise 'False'.</returns>
        bool ValueIsString();
    }
}
