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
        /// Gets the first value from the attributed <see cref="IValueAttribute"/> of the <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TOutput">The type of the underlying value.</typeparam>
        /// <typeparam name="TAtt">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public TOutput GetAttributeValue<TOutput, TAtt>(Enum enumValue)
            where TAtt : Attribute, IValueAttribute
        {
            //TAtt attribute = this.GetAttributeFromEnum<TAtt>(enumValue);
            IEnumerable<TAtt> attributes = GetAttributesFromEnum<TAtt>(enumValue);

            TAtt first = attributes?.FirstOrDefault();

            return first is null ? default : first.GetAs<TOutput>();
        }

        public object GetAttributeValue<TAtt>(Enum enumValue)
            where TAtt : Attribute, IValueAttribute
        {
            IEnumerable<TAtt> attributes = GetAttributesFromEnum(enumValue);

        }

        /// <summary>
        /// Retrieves the secondary values from one or multiple <see cref="IValueAttribute"/> attributes attached
        /// to a single <see cref="Enum"/> value type.
        /// </summary>
        /// <typeparam name="TOutput">The type of the retrieved value.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="enumValue">The specific enumeration value that has the attached <see cref="IValueAttribute"/> attributes.</param>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public IEnumerable<TOutput> GetAttributeValues<TOutput, TAtt>(Enum enumValue)
            where TAtt : Attribute, IValueAttribute
        {
            IEnumerable<TAtt> attributes = GetAttributesFromEnum<TAtt>(enumValue);
            foreach (TAtt attribute in attributes)
            {
                if (!attribute.ValueIsString() && attribute.Value is IEnumerable ienum)
                {
                    foreach (TOutput item in ienum)
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return attribute.GetAs<TOutput>();
                }
            }
        }

        public bool TryGetAttributeValue<TOutput, TAtt>(Enum enumValue, out TOutput outValue, out Exception caughtException)
            where TAtt : Attribute, IValueAttribute
        {
            caughtException = null;
            outValue = default;
            try
            {
                outValue = this.GetAttributeValue<TOutput, TAtt>(enumValue);
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
        ///     The type of <see cref="Enum"/> whose attributes that are of the type <typeparamref name="TAtt"/>
        ///     and whose <see cref="IValueAttribute.Value"/> is tested for equality against <paramref name="objValue"/>.
        /// </typeparam>
        /// <typeparam name="TAtt">
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
        public TEnum GetEnumFromValue<TEnum, TAtt>(object objValue)
            where TEnum : Enum
            where TAtt : Attribute, IValueAttribute
        {
            if (objValue is null)
                throw new ArgumentNullException(nameof(objValue));

            IEnumerable<(TAtt, TEnum)> attsAndEnums = GetAttributesFromAllEnums<TAtt, TEnum>();

            TEnum e = default;

            foreach ((TAtt, TEnum) tuple in attsAndEnums)
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
        /// Get the enum values that have attached an <see cref="IValueAttribute"/> attribute which contains any overlapping 
        /// items of the specified collection.
        /// </summary>
        /// <typeparam name="TInput">The type of the collection's items.</typeparam>
        /// <typeparam name="TOutput">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValues">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public IEnumerable<TOutput> GetEnumsFromValues<TOutput, TAtt, TInput>(IEnumerable<TInput> objValues)
            where TOutput : Enum
            where TAtt : Attribute, IValueCollection
        {
            foreach ((TAtt, TOutput) info in GetAttributesFromAllEnums<TAtt, TOutput>())
            {
                bool result = false;
                try
                {
                    result = info.Item1.GetValues<TInput>().Any(x => objValues.Contains(x));
                }
                catch
                {
                    result = false;
                }

                if (result)
                {
                    yield return info.Item2;
                }
            }
        }

        /// <summary>
        /// Get the enum value that has an <see cref="IValueAttribute"/> attribute with the matching collection
        /// of values.  If more than one <see cref="Enum"/> matches the collection, the first, lowest value <see cref="Enum"/>
        /// is returned.
        /// </summary>
        /// <typeparam name="TInput">The type of the collection's items.</typeparam>
        /// <typeparam name="TOutput">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValues">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public TOutput GetEnumFromValues<TOutput, TAtt, TInput>(IEnumerable<TInput> objValues)
            where TOutput : Enum
            where TAtt : Attribute, IValueCollection
        {
            TOutput output = default;
            foreach ((TAtt, TOutput) info in GetAttributesFromAllEnums<TAtt, TOutput>().OrderBy(x => x.Item2))
            {
                bool result = false;
                try
                {
                    result = info.Item1.GetValues<TInput>().All(x => objValues.Contains(x));
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
        private static IEnumerable<TEnum> GetEnumValues<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))?.Cast<TEnum>();
        }
        private static IEnumerable<TEnum> GetEnumValues<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))?.Cast<TEnum>();
        }

        #endregion

        ///// <summary>
        ///// Retrieves the custom attribute 
        ///// </summary>
        ///// <typeparam name="TAtt"></typeparam>
        ///// <param name="e"></param>
        ///// <returns></returns>
        //private static TAtt GetAttributeFromEnum<TAtt>(Enum e)
        //    where TAtt : Attribute
        //{
        //    return 
        //        GetFieldInfo(e)
        //            ?.GetCustomAttribute<TAtt>();
        //}

        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        /// <returns>
        ///     A collection of the attributes that are applied to <paramref name="e"/> and that match
        ///     <typeparamref name="TAtt"/>, or an empty collection.
        /// </returns>
        private static List<TAtt> GetAttributesFromEnum<TAtt>(Enum e)
            where TAtt : Attribute
        {
            IEnumerable<TAtt> atts = GetFieldInfo(e)?.GetCustomAttributes<TAtt>();

            if (atts is null)
                return new List<TAtt>();

            return new List<TAtt>(atts);
        }

        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        private static IEnumerable<(TAtt, TEnum)> GetAttributesFromAllEnums<TAtt, TEnum>()
            where TAtt : Attribute
            where TEnum : Enum
        {
            foreach (TEnum e in GetEnumValues<TEnum>())
            {
                foreach (TAtt att in GetAttributesFromEnum<TAtt>(e))
                {
                    yield return (att, e);
                }
            }
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
    }
}
