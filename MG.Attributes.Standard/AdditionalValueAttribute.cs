using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.Attributes
{
    /// <summary>
    /// An <see cref="Attribute"/> base class implementation designed for storing an additional 
    /// <see cref="object"/> value for a member (field, property, or <see cref="Enum"/>).
    /// </summary>
    /// <remarks>
    ///     This value held by this <see cref="Attribute"/> can be retrieved
    ///     by <see cref="AttributeValuator"/> or another class implementing <see cref="IAttributeValueResolver"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property, AllowMultiple = true)]
    public class AdditionalValueAttribute : Attribute, IValueAttribute, IAttributeValueCollection, IComparable<AdditionalValueAttribute>
    {
        #region PRIVATE FIELDS/CONSTANTS
        //private int _valueCount => this.BackingValue is null ? 0 : 1;
        private int _colCount => (this.BackingValueAsCollection?.Count).GetValueOrDefault();

        #endregion

        #region PROTECTED OVERRIDABLE PROPERTIES
        protected virtual object BackingValue { get; set; }
        protected virtual ICollection BackingValueAsCollection { get; set; }
        protected virtual Type BackingValueType { get; set; }

        #endregion

        #region INTERFACE EXPLICIT PROPERTIES
        object IValueAttribute.Value => this.GetValue();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The number of individual values held by the <see cref="AdditionalValueAttribute"/>.
        /// </summary>
        /// <returns>
        ///     The number of values held by the attribute.  If the value does not implement <see cref="ICollection"/> nor 
        ///     <see cref="IEnumerable"/>, 1 is returned instead.
        /// </returns>
        public virtual int Count => _colCount;
        
        /// <summary>
        /// The resolved type of the held object by the <see cref="AdditionalValueAttribute"/>.
        /// </summary>
        public Type ValueType => this.BackingValueType;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// The lone constructor for <see cref="AdditionalValueAttribute"/>.  It requires a single <see cref="object"/>
        /// as a parameter which becomes the underlying value that can be retrieved later.
        /// </summary>
        /// <param name="value">The value to hold as an additional object.</param>
        public AdditionalValueAttribute(object value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            this.BackingValueType = value.GetType();
            this.BackingValue = value;

            if (IsCollection(value, out ICollection icol))
            {
                this.BackingValueAsCollection = icol;
            }
            else if (value is IEnumerable ienum && !(value is string))
            {
                this.BackingValueAsCollection = ienum.Cast<object>().ToArray();
            }
            else
            {
                this.BackingValueAsCollection = new object[1] { value };
            }
        }

        #endregion

        #region METHODS

        public int CompareTo(AdditionalValueAttribute other)
        {
            if (this.BackingValueType.GetTypeInfo().IsValueType && other.BackingValueType.GetTypeInfo().IsValueType)
                return Comparer<ValueType>.Default.Compare((ValueType)this.GetValue(), (ValueType)other.GetValue());

            else if (this.BackingValueType == other.BackingValueType)
            {
                Type genCtor = typeof(Comparer<>).MakeGenericType(this.BackingValueType);
                PropertyInfo pi = genCtor.GetRuntimeProperty(nameof(Comparer<object>.Default));
                object defaultComparer = pi.GetValue(null);

                MethodInfo compareMeth = defaultComparer.GetType().GetRuntimeMethod(nameof(Comparer<object>.Default.Compare),
                    new Type[] { this.BackingValueType, this.BackingValueType });

                object result = compareMeth.Invoke(defaultComparer, new object[] { this.GetValue(), other.GetValue() });
                if (result is int number)
                    return number;

                else
                    throw new InvalidOperationException();
            }
            else
            {
                return Comparer<object>.Default.Compare(this.GetValue(), other.GetValue());
            }
        }

        /// <summary>
        /// Returns the held value of the <see cref="AdditionalValueAttribute"/> and casts it to the 
        /// specified type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        ///     Dynamics are not supported.
        /// </remarks>
        /// <typeparam name="T">
        ///     The <see cref="Type"/> to cast the result of <see cref="GetValue()"/>.
        /// </typeparam>
        /// <exception cref="InvalidCastException">
        ///     Thrown when the object value
        ///     can't be explicitly cast as the given type.  
        /// </exception>
        public T GetAs<T>() => (T)this.GetValue();

        /// <summary>
        /// Attempts to cast the result of <see cref="GetValue()"/>
        /// as an implementation of <see cref="ICollection"/>.
        /// </summary>
        /// <returns>
        ///     If the value provided to the <see cref="AdditionalValueAttribute"/> implements <see cref="ICollection"/>,
        ///     then a collection is returned.  If the value provided only implements <see cref="IEnumerable"/>, then an 
        ///     attempt will be to enumerate the value and return a constructed collection.
        ///     If the conditions described are <see langword="false"/>, then <see langword="null"/> is returned.
        /// </returns>
        /// <exception cref="InvalidCastException">
        ///     The result of <see cref="GetValue"/> could not be cast to 
        ///     <see cref="IEnumerable{T}"/> of the type <see cref="object"/>.
        /// </exception>
        public virtual ICollection GetValueCollection()
        {
            return this.BackingValueAsCollection;
        }

        /// <summary>
        /// Retrieves the held value as an implementation of <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to cast the result of <see cref="GetValue()"/>.</typeparam>
        /// <returns></returns>
        public virtual T[] GetValues<T>()
        {
            T[] tArr = new T[0];
            if (this.BackingValueAsCollection?.Count > 0)
            {
                tArr = this.BackingValueAsCollection.OfType<T>().ToArray();
            }
            else if (this.Count > 0)
            {
                try
                {
                    T one = this.GetAs<T>();
                    tArr = new T[1] { one };
                }
                catch
                {
                }
            }

            return tArr;
        }


        /// <summary>
        /// Attempts to retrieve and cast the result of <see cref="GetValue()"/> as the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        ///     Any <see cref="Exception"/> that occurs will be discarded.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> to cast the result of <see cref="GetValue()"/>.</typeparam>
        /// <param name="tIsValidFunc">
        ///     The function used to validate that the cast of <see cref="GetValue()"/> results in 
        ///     the desired output.
        /// </param>
        /// <param name="valueAsT">
        ///     The casted value output from <see cref="GetValue()"/>.  Even if the <paramref name="tIsValidFunc"/> function
        ///     returns <see langword="false"/>, this value can still be populated.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="bool"/> result value of the <paramref name="tIsValidFunc"/> function.
        ///     -or-
        ///     Returns <see langword="false"/> if an <see cref="Exception"/> occurs.
        /// </returns>
        public bool TryGetAs<T>(Func<T, bool> tIsValidFunc, out T valueAsT)
        {
            return this.TryGetAs(tIsValidFunc, out valueAsT, out Exception throwAway);
        }

        /// <summary>
        /// Attempts to retrieve and cast the result of <see cref="GetValue()"/> as the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        ///     Any <see cref="Exception"/> that is thrown is captured in <paramref name="caughtException"/>.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> to cast the result of <see cref="GetValue()"/>.</typeparam>
        /// <param name="tIsValidFunc">
        ///     The function used to validate that the cast of <see cref="GetValue()"/> results in 
        ///     the desired output.
        /// </param>
        /// <param name="valueAsT">
        ///     The casted value output from <see cref="GetValue()"/>.  Even if the <paramref name="tIsValidFunc"/> function
        ///     returns <see langword="false"/>, this value can still be populated.
        /// </param>
        /// <param name="caughtException">
        ///     An out variable holding any captured <see cref="Exception"/> that may have resulted during the call.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="bool"/> result value of the <paramref name="tIsValidFunc"/> function.
        ///     -or-
        ///     Returns <see langword="false"/> if an <see cref="Exception"/> occurs.
        /// </returns>
        public bool TryGetAs<T>(Func<T, bool> tIsValidFunc, out T valueAsT, out Exception caughtException)
        {
            valueAsT = default;
            caughtException = null;
            object value = this.GetValue();

            if (value is T tVal)
                valueAsT = tVal;

            else
            {
                try
                {
                    valueAsT = (T)value;
                }
                catch (Exception e)
                {
                    caughtException = e;
                    return false;
                }
            }

            return tIsValidFunc(valueAsT);
        }
        /// <summary>
        /// Attempts to retrieve the resut of <see cref="GetValueCollection()"/> as an implementation of <see cref="ICollection"/>.
        /// </summary>
        /// <remarks>
        ///     Any <see cref="Exception"/> that is thrown is discarded.
        /// </remarks>
        /// <param name="collection">The out variable of the value as an implementation of <see cref="ICollection"/>.</param>
        /// <returns>
        ///     <see langword="true"/> if <paramref name="collection"/> is not <see langword="null"/>;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetAsCollection(out ICollection collection)
        {
            return this.TryGetAsCollection(out collection, out Exception caughtException);
        }
        /// <summary>
        /// Attempts to retrieve the resut of <see cref="GetValueCollection()"/> as an implementation of <see cref="ICollection"/>.
        /// </summary>
        /// <remarks>
        ///     Any <see cref="Exception"/> that is thrown is captured in <paramref name="caughtException"/>.
        /// </remarks>
        /// <param name="collection">The out variable of the value as an implementation of <see cref="ICollection"/>.</param>
        /// <param name="caughtException">
        ///     An out variable holding any captured <see cref="Exception"/> that may have resulted during the call.
        /// </param>
        /// <returns>
        ///     <see langword="false"/> if <paramref name="collection"/> is <see langword="null"/> or 
        ///     an <see cref="Exception"/> was caught in <paramref name="caughtException"/>; otherwise, 
        ///     <see langword="true"/>.
        /// </returns>
        public bool TryGetAsCollection(out ICollection collection, out Exception caughtException)
        {
            collection = null;
            caughtException = null;
            try
            {
                collection = this.GetValueCollection();
                return null != collection;
            }
            catch (Exception e)
            {
                caughtException = e;
                return false;
            }
        }
        public bool ValueIsString() => typeof(string).Equals(this.BackingValueType);

        #region ENUMERATORS
        /// <summary>
        /// Returns an enumerator that iterates through the held values like a collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            if (this.TryGetAsCollection(out ICollection icol))
            {
                yield return icol.GetEnumerator();
            }
            else
                yield return this.GetValue();
        }

        #endregion

        #region PROTECTED OVERRIDABLE METHODS
        /// <summary>
        /// The main method of retrieving the held value from the <see cref="AdditionalValueAttribute"/>.
        /// </summary>
        /// <remarks>
        ///     The interface explicit implementation of <see cref="IValueAttribute.Value"/> retrieves the
        ///     value using this method.
        /// </remarks>
        /// <returns>
        ///     The held value of the <see cref="AdditionalValueAttribute"/> as an <see cref="object"/>.
        ///     By default, it returns the value of <see cref="BackingValue"/>.
        /// </returns>
        protected virtual object GetValue() => this.BackingValue;

        /// <summary>
        /// The method called in the default constructor used to resolve the <see cref="Type"/> of the 
        /// incoming <see cref="object"/>.
        /// </summary>
        /// <remarks>
        ///     Can be overriden for custom <see cref="Type"/> resolution.
        /// </remarks>
        /// <param name="value">The <see cref="AdditionalValueAttribute"/>'s held <see cref="object"/>.</param>
        /// <returns>
        ///     By default, <paramref name="value"/>'s <see cref="Type"/> resolved by <see cref="object.GetType()"/>
        /// </returns>
        protected virtual Type GetValueType(object value)
        {
            return value.GetType();
        }

        #endregion

        #region PRVIATE METHODS
        private static bool IsCollection(object value, out ICollection icol)
        {
            icol = null;
            if (value is ICollection col)
            {
                icol = col;
            }

            return null != icol;
        }

        #endregion

        #endregion
    }
}
