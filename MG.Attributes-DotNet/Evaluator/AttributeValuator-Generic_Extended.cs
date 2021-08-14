using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.Attributes
{
    public partial class AttributeValuator<TAttribute, TOutput>
    {
        private const BindingFlags DEFAULT_FLAGS = BindingFlags.Public | BindingFlags.Instance;

        public IEnumerable<TOutput> GetValues<TClass>(BindingFlags flags, bool includeFields = false)
            where TClass : class
        {
            Type tClass = typeof(TClass);
            IEnumerable<MemberInfo> members = tClass.GetProperties(flags);
            if (includeFields)
            {
                members = members.Concat(tClass.GetFields(flags));
            }

            return this.GetValues(members);
        }
    }
}
