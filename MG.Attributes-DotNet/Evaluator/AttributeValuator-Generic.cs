using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.Attributes
{
    public partial class AttributeValuator<TAttribute, TOutput> where TAttribute : Attribute
    {
        private Func<TAttribute, TOutput> _singleValueFunction;
        private Func<TAttribute, IEnumerable<TOutput>> _multipleValueFunction;

        public AttributeValuator(Func<TAttribute, TOutput> valueFunction)
        {
            _singleValueFunction = valueFunction;
            _multipleValueFunction = x => new TOutput[] { _singleValueFunction(x) };
        }
        public AttributeValuator(Func<TAttribute, TOutput> singleValueFunction, Func<TAttribute, IEnumerable<TOutput>> multiValueFunction)
        {
            _singleValueFunction = singleValueFunction;
            _multipleValueFunction = multiValueFunction;
        }

        public TOutput GetValue(MemberInfo memberInfo)
        {
            ValidateArgument(memberInfo);

            TAttribute att = memberInfo.GetCustomAttribute<TAttribute>();
            if (null == att)
                return default;

            return _singleValueFunction(att);
        }
        public TOutput GetValue<TClass, TProp>(Expression<Func<TClass, TProp>> memberExpression)
        {
            if (TryAsMemberExpression(memberExpression, out MemberExpression memEx))
            {
                return this.GetValue(memEx.Member);
            }
            else
            {
                return default;
            }
        }

        public IEnumerable<TOutput> GetMultiValue(MemberInfo memberInfo)
        {
            ValidateArgument(memberInfo);

            TAttribute att = memberInfo.GetCustomAttribute<TAttribute>();
            if (null == att)
                return default;

            return _multipleValueFunction(att);
        }
        public IEnumerable<TOutput> GetMultiValue<TClass, TProp>(Expression<Func<TClass, TProp>> memberExpression)
        {
            if (TryAsMemberExpression(memberExpression, out MemberExpression memEx))
            {
                return this.GetMultiValue(memEx.Member);
            }
            else
            {
                return null;
            }
        }

        public List<TOutput> GetMultiValues<TClass, TProp>(params Expression<Func<TClass, TProp>>[] expressions)
        {
            if (null == expressions || expressions.Length <= 0)
                return null;

            var list = new List<TOutput>(expressions.Length * 2);

            for (int i = 0; i < expressions.Length; i++)
            {
                IEnumerable<TOutput> results = this.GetMultiValue(expressions[i]);
                if (null != results)
                    list.AddRange(results);
            }

            return list;
        }
        public IEnumerable<TOutput> GetValues<TMember>(IEnumerable<TMember> memberInfos)
            where TMember : MemberInfo
        {
            ValidateArgument(memberInfos);

            foreach (TMember info in memberInfos)
            {
                foreach (TAttribute tAtt in GetAttributes(info))
                {
                    foreach (TOutput value in _multipleValueFunction(tAtt))
                    {
                        yield return value;
                    }
                }
            }
        }

        private static IEnumerable<TAttribute> GetAttributes(MemberInfo memberInfo)
        {
            foreach (TAttribute tAtt in memberInfo.GetCustomAttributes<TAttribute>())
            {
                yield return tAtt;
            }
        }
        private static bool TryGetAttribute(MemberInfo memberInfo, out TAttribute propValueAtt)
        {
            propValueAtt = memberInfo.GetCustomAttributes<TAttribute>().FirstOrDefault();
            return null != propValueAtt;
        }
        private static bool TryAsMemberExpression<TClass, TMember>(Expression<Func<TClass, TMember>> expression, out MemberExpression member)
        {
            member = null;

            if (expression?.Body is MemberExpression memEx)
            {
                member = memEx;
            }
            else if (expression?.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
            {
                member = unExMem;
            }

            return null != member;
        }
        private static void ValidateArgument<T>(T argument)
            where T : class
        {
            if (null == argument)
            {
                throw new ArgumentNullException(nameof(argument));
            }
        }
        private static void ValidateArgument<T>(IEnumerable<T> argument)
            where T : class
        {
            if (null == argument)
            {
                throw new ArgumentNullException(nameof(argument));
            }
        }
    }
}
