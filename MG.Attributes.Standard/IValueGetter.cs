using System;
using System.Collections.Generic;
using System.Reflection;

namespace MG.Attributes
{
    public interface IValueGetter
    {
        T1 GetAttributeValue<T1, T2, T3>(T2 enumValue)
            where T2 : Enum
            where T3 : Attribute, IValueAttribute;

        IEnumerable<T1> GetAttributeValues<T1, T2, T3>(T2 enumValue)
            where T2 : Enum
            where T3 : Attribute, IValueAttribute;
    }
}
