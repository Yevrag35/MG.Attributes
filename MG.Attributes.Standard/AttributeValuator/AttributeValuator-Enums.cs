using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Attributes
{
    public partial class AttributeValuator
    {

        #region ATTRIBUTES METHODS
        /// <summary>
        /// Gets the value from the attributed <see cref="IValueAttribute.Value"/> of the <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the underlying value.</typeparam>
        /// <typeparam name="T2">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public T1 GetAttributeValue<T1, T2>(Enum enumValue)
            where T2 : Attribute, IValueAttribute
        {
            T2 attribute = this.GetAttributeFromEnum<T2>(enumValue);
            return attribute.GetAs<T1>();
        }

        /// <summary>
        /// Retrieves the secondary values from one or multiple <see cref="IValueAttribute"/> attributes attached
        /// to a single <see cref="Enum"/> value type.
        /// </summary>
        /// <typeparam name="T1">The type of the retrieved value.</typeparam>
        /// <typeparam name="T2">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="enumValue">The specific enumeration value that has the attached <see cref="IValueAttribute"/> attributes.</param>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public IEnumerable<T1> GetAttributeValues<T1, T2>(Enum enumValue)
            where T2 : Attribute, IValueAttribute
        {
            IEnumerable<T2> attributes = this.GetAttributesFromEnum<T2>(enumValue);
            foreach (T2 attribute in attributes)
            {
                if (!attribute.ValueIsString() && attribute.Value is IEnumerable ienum)
                {
                    foreach (T1 item in ienum)
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return attribute.GetAs<T1>();
                }
            }
        }
        public bool TryGetAttributeValue<T1, T2>(Enum enumValue, out T1 outValue)
            where T2 : Attribute, IValueAttribute
        {
            T2 attribute = this.GetAttributeFromEnum<T2>(enumValue);
            outValue = attribute.GetAs<T1>();
            return outValue != null;
        }

        #endregion

        #region ENUM METHODS
        public T2 GetEnumFromValue<T1, T2, T3>(T1 objVal)
            where T2 : Enum
            where T3 : Attribute, IValueAttribute
        {
            return GetEnumValues<T2>()
                .Single(att =>
                    GetAttributeValue<T1, T3>(att).Equals(objVal));
        }
        public IEnumerable<T2> GetEnumsFromValues<T1, T2, T3>(IEnumerable<T1> objVals)
            where T2 : Enum
            where T3 : Attribute, IValueAttribute
        {
            return GetEnumValues<T2>()
                .Where(att =>
                    GetAttributeValues<T1, T3>(att)
                        .Any(item =>
                            objVals.Contains(item)));
        }
        /// <summary>
        /// Get the enum value that has the <see cref="IValueAttribute"/> with the matching collection
        /// of values.
        /// </summary>
        /// <typeparam name="T1">The type of the collection's items.</typeparam>
        /// <typeparam name="T2">The type of <see cref="Enum"/> that will be searched.</typeparam>
        /// <typeparam name="T3">The type of </typeparam>
        /// <param name="objVals">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="InvalidOperationException"/>
        public T2 GetEnumFromValues<T1, T2, T3>(IEnumerable<T1> objVals)
            where T2 : Enum
            where T3 : Attribute, IValueAttribute
        {
            return GetEnumValues<T2>()
                .Single(att =>
                    GetAttributeValues<T1, T3>(att)
                        .All(item =>
                            objVals.Contains(item)));
        }

        #endregion


        #region ENUM VALUES
        private IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        private IEnumerable<T> GetEnumValues<T>(T enumValue) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        #endregion

        private T3 GetAttributeFromEnum<T3>(Enum e)
            where T3 : Attribute, IValueAttribute
        {
            return this.GetFieldInfo(e)?.GetCustomAttribute<T3>();
        }
        private IEnumerable<T3> GetAttributesFromEnum<T3>(Enum e)
            where T3 : Attribute, IValueAttribute
        {
            return this.GetFieldInfo(e)?.GetCustomAttributes<T3>();
        }
        private FieldInfo GetFieldInfo(Enum e) => e.GetType().GetRuntimeField(e.ToString());
    }
}
