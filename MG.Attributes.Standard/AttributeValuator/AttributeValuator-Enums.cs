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

        public bool TryGetAttributeValue<T1, T2>(Enum enumValue, out T1 outValue, out Exception caughtException)
            where T2 : Attribute, IValueAttribute
        {
            caughtException = null;
            outValue = default;
            try
            {
                outValue = this.GetAttributeValue<T1, T2>(enumValue);
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
        /// Get the enum value that has an <see cref="IValueAttribute"/> attribute with the matching value.
        /// </summary>
        /// <typeparam name="T1">The type of the value that is supplied.</typeparam>
        /// <typeparam name="T2">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="T3">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValue">The object that <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <exception cref="ArgumentNullException">The source or predicate is null.</exception>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The method is invoked by reflection in a reflection-only context, -or-enumType
        ///     is a type from an assembly loaded in a reflection-only context.
        ///     
        ///     OR
        ///     
        ///     No element satisfies the condition in predicate.-or-More than one element satisfies
        ///     the condition in predicate.-or-The source sequence is empty.
        /// </exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public T2 GetEnumFromValue<T1, T2, T3>(T1 objValue)
            where T2 : Enum
            where T3 : Attribute, IValueAttribute
        {
            return GetEnumValues<T2>()
                .Single(att =>
                    GetAttributeValue<T1, T3>(att).Equals(objValue));
        }

        /// <summary>
        /// Get the enum values that have attached an <see cref="IValueAttribute"/> attribute which contains any overlapping 
        /// items of the specified collection.
        /// </summary>
        /// <typeparam name="T1">The type of the collection's items.</typeparam>
        /// <typeparam name="T2">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="T3">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValues">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The method is invoked by reflection in a reflection-only context, -or-enumType
        ///     is a type from an assembly loaded in a reflection-only context.
        /// </exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public IEnumerable<T2> GetEnumsFromValues<T1, T2, T3>(IEnumerable<T1> objValues)
            where T2 : Enum
            where T3 : Attribute, IValueAttribute
        {
            return GetEnumValues<T2>()
                ?.Where(att =>
                    GetAttributeValues<T1, T3>(att)
                        .Any(item =>
                            objValues.Contains(item)));
        }

        /// <summary>
        /// Get the enum value that has an <see cref="IValueAttribute"/> attribute with the matching collection.
        /// of values.
        /// </summary>
        /// <typeparam name="T1">The type of the collection's items.</typeparam>
        /// <typeparam name="T2">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="T3">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <param name="objValues">The collection of items the <see cref="IValueAttribute.Value"/> should equal.</param>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The method is invoked by reflection in a reflection-only context, -or-enumType
        ///     is a type from an assembly loaded in a reflection-only context.
        ///     
        ///     OR
        ///     
        ///     No element satisfies the condition in predicate.-or-More than one element satisfies
        ///     the condition in predicate.-or-The source sequence is empty.
        /// </exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public T2 GetEnumFromValues<T1, T2, T3>(IEnumerable<T1> objValues)
            where T2 : Enum
            where T3 : Attribute, IValueAttribute
        {
            return GetEnumValues<T2>()
                .Single(att =>
                    GetAttributeValues<T1, T3>(att)
                        .All(item =>
                            objValues.Contains(item)));
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
