using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.Attributes
{
    public partial class AttributeValuator : IValueEnumGetter//, IValueMemberGetter
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
        /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public T1 GetAttributeValue<T1, T2, T3, T4>(Expression<Func<T3, T4>> memberExpression)
            where T2 : Attribute, IValueAttribute
        {
            MemberInfo memInfo = this.GetMemberInfo(memberExpression);
            T1 outValue = default;
            if (memInfo != null)
            {
                T2 attribute = memInfo.GetCustomAttribute<T2>();
                if (attribute != null)
                {
                    outValue = attribute.GetAs<T1>();
                }
            }
            return outValue;
        }

        /// <summary>
        /// Retrieves the secondary values from one or multiple <see cref="IValueAttribute"/> attributes attached to 
        /// the specified member specified by an <see cref="Expression"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the retrieved value.</typeparam>
        /// <typeparam name="T2">The type of <see cref="IValueAttribute"/> that is attached.</typeparam>
        /// <typeparam name="T3">The type of object for the member expression.</typeparam>
        /// <typeparam name="T4">The member type of the expression.</typeparam>
        /// <param name="memberExpression">The <see cref="Expression"/> pointing to a object's member to search for attributes.</param> 
        /// <exception cref="InvalidCastException">The secondary value cannot be converted to the specified type.</exception>
        /// <exception cref="NotSupportedException">Element is not a constructor, method, property, event, type, or field.</exception>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
        public IEnumerable<T1> GetAttributeValues<T1, T2, T3, T4>(Expression<Func<T3, T4>> memberExpression)
            where T2 : Attribute, IValueAttribute
        {
            IEnumerable<T1> outEnum = null;
            MemberInfo memInfo = this.GetMemberInfo(memberExpression);
            if (memInfo != null)
            {
                outEnum = this.GetMemberValuesFromArray<T1, T2>(memInfo);
            }
            return outEnum;
        }

        private MemberInfo GetMemberInfo<T, U>(Expression<Func<T, U>> expression)
        {
            MemberInfo memInfo = null;
            if (expression.Body is MemberExpression memEx)
            {
                memInfo = memEx.Member;
            }
            else if (expression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
            {
                memInfo = unExMem.Member;
            }
            return memInfo;
        }
        private IEnumerable<T1> GetMemberValues<T1, T2>(IEnumerable<MemberInfo> memberInfos)
            where T2 : Attribute, IValueAttribute
        {
            foreach (MemberInfo mi in memberInfos)
            {
                foreach (T2 attribute in mi.GetCustomAttributes<T2>())
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
        }
        private IEnumerable<T1> GetMemberValuesFromArray<T1, T2>(params MemberInfo[] memberInfos)
            where T2 : Attribute, IValueAttribute
        {
            if (memberInfos == null || memberInfos.Length <= 0)
                return null;

            return this.GetMemberValues<T1, T2>(memberInfos);
        }

        //public MemberExpression GetMemberFromValue<T1, T2, T3>(T1 objValue)
        //    where T2 : Attribute, IValueAttribute
        //{
        //    Type t3Type = typeof(T3);
        //    IEnumerable<MemberInfo> memberInfos = t3Type
        //        .GetRuntimeFields()
        //            .Cast<MemberInfo>()
        //                .Concat(t3Type.GetRuntimeProperties());

        //    IEnumerable<T1> allValues = this.GetMemberValues<T1, T2>(memberInfos);
        //    if (allValues != null && allValues.Any(v => v.Equals(objValue)))
        //    {

        //    }
        //}
    }
}
