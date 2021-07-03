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
        /// Gets the value from the attributed <see cref="IValueAttribute.Value"/> of the <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TOutput">The type of the underlying value.</typeparam>
        /// <typeparam name="TAtt">The type of the <see cref="IValueAttribute"/>.</typeparam>
        /// <param name="enumValue">The <see cref="Enum"/> whose value is retrieved.</param>
        /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public TOutput GetAttributeValue<TOutput, TAtt>(Enum enumValue)
            where TAtt : Attribute, IValueAttribute
        {
            TAtt attribute = this.GetAttributeFromEnum<TAtt>(enumValue);
            return attribute.GetAs<TOutput>();
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
            IEnumerable<TAtt> attributes = this.GetAttributesFromEnum<TAtt>(enumValue);
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
        /// Get the enum value that has an <see cref="IValueAttribute"/> attribute with the matching value.
        /// </summary>
        /// <typeparam name="TInput">The type of the value that is supplied.</typeparam>
        /// <typeparam name="TEnum">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
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
        public TEnum GetEnumFromValue<TEnum, TAtt>(object objValue)
            where TEnum : Enum
            where TAtt : Attribute, IValueAttribute
        {
            IEnumerable<(TAtt, TEnum)> attsAndEnums = this
                .GetAttributesFromAllEnums<TAtt, TEnum>();

            TEnum e = default;

            foreach (var tuple in attsAndEnums)
            {
                if (tuple.Item1.Value.Equals(objValue))
                {
                    e = tuple.Item2;
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
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The method is invoked by reflection in a reflection-only context, -or-enumType
        ///     is a type from an assembly loaded in a reflection-only context.
        /// </exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public IEnumerable<TOutput> GetEnumsFromValues<TInput, TOutput, TAtt>(IEnumerable<TInput> objValues)
            where TOutput : Enum
            where TAtt : Attribute, IValueAttribute
        {
            return GetEnumValues<TOutput>()
                ?.Where(att =>
                    GetAttributeValues<TInput, TAtt>(att)
                        .Any(item =>
                            objValues.Contains(item)));
        }

        /// <summary>
        /// Get the enum value that has an <see cref="IValueAttribute"/> attribute with the matching collection.
        /// of values.
        /// </summary>
        /// <typeparam name="TInput">The type of the collection's items.</typeparam>
        /// <typeparam name="TOutput">The type of <see cref="Enum"/> that will be returned.</typeparam>
        /// <typeparam name="TAtt">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
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
        public TOutput GetEnumFromValues<TInput, TOutput, TAtt>(IEnumerable<TInput> objValues)
            where TOutput : Enum
            where TAtt : Attribute, IValueAttribute
        {
            return GetEnumValues<TOutput>()
                .Single(att =>
                    GetAttributeValues<TInput, TAtt>(att)
                        .All(item =>
                            objValues.Contains(item)));
        }

        #endregion


        #region ENUM VALUES
        private IEnumerable<TEnum> GetEnumValues<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }
        private IEnumerable<TEnum> GetEnumValues<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        #endregion

        private TAtt GetAttributeFromEnum<TAtt>(Enum e)
            where TAtt : Attribute
        {
            return this.GetFieldInfo(e)?.GetCustomAttribute<TAtt>();
        }
        private IEnumerable<TAtt> GetAttributesFromEnum<TAtt>(Enum e)
            where TAtt : Attribute
        {
            return this.GetFieldInfo(e)?.GetCustomAttributes<TAtt>();
        }
        private IEnumerable<(TAtt, TEnum)> GetAttributesFromAllEnums<TAtt, TEnum>()
            where TAtt : Attribute
            where TEnum : Enum
        {
            foreach (TEnum e in Enum.GetValues(typeof(TEnum)))
            {
                foreach (TAtt att in this.GetAttributesFromEnum<TAtt>(e))
                {
                    yield return (att, e);
                }
            }
        }
        private FieldInfo GetFieldInfo(Enum e) => e.GetType().GetRuntimeField(e.ToString());
    }
}
