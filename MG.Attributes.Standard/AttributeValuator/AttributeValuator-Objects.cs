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
    }
}
