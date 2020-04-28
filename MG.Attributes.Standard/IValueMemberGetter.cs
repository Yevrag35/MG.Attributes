using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MG.Attributes
{
    /// <summary>
    /// An interface exposing methods for retrieving secondary object values from 
    /// class members.
    /// </summary>
    public interface IValueMemberGetter
    {
        /// <summary>
        /// Retrieves a secondary value from an <see cref="IValueAttribute"/> attribute attached to 
        /// the specified member specified by an <see cref="Expression"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the retrieved value.</typeparam>
        /// <typeparam name="T2">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <typeparam name="T3">The type of object for the member expression.</typeparam>
        /// <typeparam name="T4">The member type of the expression.</typeparam>
        /// <param name="memberExpression">The <see cref="Expression"/> pointing to a object's member to search for the attribute.</param>
        T1 GetAttributeValue<T1, T2, T3, T4>(Expression<Func<T3, T4>> memberExpression)
            where T2 : Attribute, IValueAttribute;

        /// <summary>
        /// Retrieves the secondary values from one or multiple <see cref="IValueAttribute"/> attributes attached to 
        /// the specified member specified by an <see cref="Expression"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the retrieved value.</typeparam>
        /// <typeparam name="T2">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <typeparam name="T3">The type of object for the member expression.</typeparam>
        /// <typeparam name="T4">The member type of the expression.</typeparam>
        /// <param name="memberExpression">The <see cref="Expression"/> pointing to a object's member to search for attributes.</param> 
        IEnumerable<T1> GetAttributeValues<T1, T2, T3, T4>(Expression<Func<T3, T4>> memberExpression)
            where T2 : Attribute, IValueAttribute;
    }
}
