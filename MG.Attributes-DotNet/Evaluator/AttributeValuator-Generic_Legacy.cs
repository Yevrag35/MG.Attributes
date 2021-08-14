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
        public IEnumerable<TOutput> GetValues<TClass>(bool includeFields = false)
            where TClass : class
        {
            Type tClass = typeof(TClass);
            IEnumerable<MemberInfo> members = tClass.GetRuntimeProperties();
            if (includeFields)
            {
                members = members.Concat(tClass.GetRuntimeFields());
            }

            return this.GetValues(members);
        }
    }
}
