using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.Attributes
{
    public partial class AttributeValuator
    {

        #region ATTRIBUTES METHODS
        /// <summary>
        /// Gets the first value from the attributed <see cref="IValueAttribute"/> of the <see cref="Enum"/>, then casts 
        /// and returns it as the type <typeparamref name="TOutput"/>.
        /// </summary>
        /// <typeparam name="TOutput">The type of the underlying value.</typeparam>
        /// <typeparam name="TAttribute">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public TOutput GetAttributeValue<TOutput, TAttribute>(Enum enumValue)
            where TAttribute : Attribute, IValueAttribute
        {
            //TAttribute attribute = this.GetAttributeFromEnum<TAttribute>(enumValue);
            IEnumerable<TAttribute> attributes = GetAttributesFromEnum<TAttribute>(enumValue);

            TAttribute first = attributes?.FirstOrDefault();

            return first is null ? default : first.GetAs<TOutput>();
        }

        /// <summary>
        /// Gets the first value from the attributed <see cref="IValueAttribute"/> of the <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public object GetAttributeValue<TAttribute>(Enum enumValue)
            where TAttribute : Attribute, IValueAttribute
        {
            IEnumerable<TAttribute> attributes = GetAttributesFromEnum<TAttribute>(enumValue);

            return attributes?.FirstOrDefault()?.Value;
        }

        /// <summary>
        /// Retrieves the secondary values from one or multiple <see cref="IValueAttribute"/> attributes attached
        /// to a single <see cref="Enum"/> value type.
        /// </summary>
        /// <typeparam name="TOutput">The type of the retrieved value.</typeparam>
        /// <typeparam name="TAttribute">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="enumValue">The specific enumeration value that has the attached <see cref="IValueAttribute"/> attributes.</param>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public TOutput[] GetAttributeValues<TAttribute, TOutput>(Enum enumValue)
            where TAttribute : Attribute, IAttributeValueCollection
        {
            List<TAttribute> attributes = GetAttributesFromEnum<TAttribute>(enumValue);
            var list = new List<TOutput>((attributes?.Count).GetValueOrDefault());

            for (int i = 0; i < attributes?.Count; i++)
            {
                list.AddRange(attributes[i].GetValues<TOutput>());
            }

            return list.ToArray();
        }

        /// <summary>
        /// Attempts to get the first value from the attributed <see cref="IValueAttribute"/> of the 
        /// <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TOutput">The type of the underlying value.</typeparam>
        /// <typeparam name="TAttribute">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose attribute's value is retrieved.</param>
        /// <param name="outValue"></param>
        /// <param name="caughtException"></param>
        /// <returns></returns>
        public bool TryGetAttributeValue<TOutput, TAttribute>(Enum enumValue, out TOutput outValue, out Exception caughtException)
            where TAttribute : Attribute, IValueAttribute
        {
            caughtException = null;
            outValue = default;
            try
            {
                outValue = this.GetAttributeValue<TOutput, TAttribute>(enumValue);
                return outValue != null;
            }
            catch (Exception e)
            {
                caughtException = e;
                return false;
            }
        }

        #endregion

        #region ENUM METHODS
        /// <summary>
        /// Gets the first, lowest enumeration that has an <see cref="IValueAttribute.Value"/> matching 
        /// the given <see cref="object"/>.
        /// </summary>
        /// <typeparam name="TEnum">
        ///     The type of <see cref="Enum"/> whose attributes that are of the type <typeparamref name="TAttribute"/>
        ///     and whose <see cref="IValueAttribute.Value"/> is tested for equality against <paramref name="objValue"/>.
        /// </typeparam>
        /// <typeparam name="TAttribute">
        ///     The type of <see cref="Attribute"/> which implements <see cref="IValueAttribute"/> that is searched
        ///     for on the specified <typeparamref name="TEnum"/>.
        /// </typeparam>
        /// <param name="objValue">The object that <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="ArgumentNullException"><paramref name="objValue"/> is null.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        /// <returns>
        ///     An <see cref="Enum"/> value of type <typeparamref name="TEnum"/> that has an <see cref="IValueAttribute"/>
        ///     implementing <see cref="Attribute"/> whose held value matches the specified <paramref name="objValue"/>.
        /// </returns>
        public TEnum GetEnumFromValue<TEnum, TAttribute>(object objValue)
            where TEnum : Enum
            where TAttribute : Attribute, IValueAttribute
        {
            if (objValue is null)
                throw new ArgumentNullException(nameof(objValue));

            IEnumerable<(TAttribute, TEnum)> attsAndEnums = GetAttributesFromAllEnums<TAttribute, TEnum>();

            TEnum e = default;

            foreach ((TAttribute, TEnum) tuple in attsAndEnums)
            {
                if (tuple.Item1.Value.Equals(objValue))
                {
                    e = tuple.Item2;
                    break;
                }
            }

            return e;
        }

        /// <summary>
        /// Returns the individual <see cref="Enum"/> values that are decorated with <see cref="IValueAttribute"/>
        /// attributes whose <see cref="IValueAttribute.Value"/> matches the specified value.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="value">The value that is equal to <see cref="IValueAttribute.Value"/>.</param>
        /// <returns>
        ///     A single-dimensional array of <typeparamref name="TEnum"/> values that equate to <paramref name="value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public TEnum[] GetEnumsFromValue<TEnum, TAttribute>(object value)
            where TEnum : Enum
            where TAttribute : Attribute, IValueAttribute
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return GetAttributesFromAllEnums<TAttribute, TEnum>()
                .FilterAndReturn(
                    tuple =>
                        value.Equals(tuple.Item1.Value),
                        tuple => tuple.Item2);
        }

        /// <summary>
        /// Get the enum values that have attached an <see cref="IValueAttribute"/> attribute which contains any overlapping 
        /// items of the specified collection.
        /// </summary>
        /// <typeparam name="TInput">The type of the collection's items.</typeparam>
        /// <typeparam name="TEnum">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAttribute">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValues">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public TEnum[] GetEnumsFromValues<TEnum, TAttribute, TInput>(IEnumerable<TInput> objValues)
            where TEnum : Enum
            where TAttribute : Attribute, IAttributeValueCollection
        {
            return GetAttributesFromAllEnums<TAttribute, TEnum>()
                .FilterAndReturn(
                    tuple => tuple.Item1.GetValues<TInput>().Overlaps(objValues),
                    tuple => tuple.Item2);
        }

        /// <summary>
        /// Get the enum value that has an <see cref="IValueAttribute"/> attribute with the matching collection
        /// of values.  If more than one <see cref="Enum"/> matches the collection, the first, lowest value <see cref="Enum"/>
        /// is returned.
        /// </summary>
        /// <typeparam name="TInput">The type of the collection's items.</typeparam>
        /// <typeparam name="TOutput">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAttribute">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="values">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="values"/> contains no non-<see langword="null"/> elements.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public TOutput GetEnumFromValues<TOutput, TAttribute, TInput>(IEnumerable<TInput> values)
            where TOutput : Enum
            where TAttribute : Attribute, IAttributeValueCollection
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            else if (!this.ContainsAnyValue(values))
                throw new ArgumentException(string.Format("{0} must contain at least 1 non-null element.", nameof(values)));

            TOutput output = default;

            foreach ((TAttribute, TOutput) info in GetAttributesFromAllEnums<TAttribute, TOutput>())
            {
                bool result = false;
                try
                {
                    TInput[] arr = info.Item1.GetValues<TInput>();
                    result = arr.Length > 0 && values.All(x => arr.Contains(x));
                }
                catch (Exception)
                {
                    result = false;
                }
                
                if (result)
                {
                    output = info.Item2;
                    break;
                }
            }

            return output;
        }

        #endregion

        #region ENUM VALUES
        private static TEnum[] GetEnumValues<TEnum>() where TEnum : Enum
        {
            Array array = Enum.GetValues(typeof(TEnum));
            TEnum[] eArr = new TEnum[(array?.Length).GetValueOrDefault()];

            if (array?.Length <= 0)
                return eArr;

            array.CopyTo(eArr, 0);
            Array.Clear(array, 0, array.Length);

            return eArr;
        }

        #endregion

        private bool ContainsAnyValue<T>(IEnumerable<T> values)
        {
            return !typeof(T).GetTypeInfo().IsValueType && !values.Any(x => null != x);
        }

        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        /// <returns>
        ///     A collection of the attributes that are applied to <paramref name="e"/> and that match
        ///     <typeparamref name="TAttribute"/>, or an empty collection.
        /// </returns>
        private static List<TAttribute> GetAttributesFromEnum<TAttribute>(Enum e)
            where TAttribute : Attribute
        {
            IEnumerable<TAttribute> atts = GetFieldInfo(e)?.GetCustomAttributes<TAttribute>();

            if (atts is null)
                return new List<TAttribute>();

            return new List<TAttribute>(atts);
        }

        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        private static List<(TAttribute, TEnum)> GetAttributesFromAllEnums<TAttribute, TEnum>()
            where TAttribute : Attribute
            where TEnum : Enum
        {
            TEnum[] enumArr = GetEnumValues<TEnum>();
            var list = new List<(TAttribute, TEnum)>(enumArr.Length);

            for (int i1 = 0; i1 < enumArr.Length; i1++)
            {
                var attList = GetAttributesFromEnum<TAttribute>(enumArr[i1]);
                for (int i2 = 0; i2 < attList.Count; i2++)
                {
                    list.Add((attList[i2], enumArr[i1]));
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves the <see cref="FieldInfo"/> for the specified enumeration value.
        /// </summary>
        /// <param name="e">
        ///     The enumeration value that is used, after being converted to <see cref="string"/>,
        ///     to return the <see cref="FieldInfo"/> by name of the <see cref="Enum"/> type.
        /// </param>
        /// <returns>
        ///     A <see cref="FieldInfo"/> object that represents the specified field of the enumeration value, or
        ///     <see langword="null"/> if the field is not found.
        /// </returns>
        private static FieldInfo GetFieldInfo(Enum e)
        {
            return 
                e.GetType()
                    .GetRuntimeField(
                        e.ToString()
                    );
        }

        private static TOutput ConvertToGeneric<TOutput>(object input) => (TOutput)input;
    }
}
