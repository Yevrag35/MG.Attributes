using System;
using System.Collections;
using System.Linq;

namespace MG.Attributes
{
    public abstract class MGAbstractAttribute : Attribute, IAttribute
    {
        private readonly object _val;
        public object Value => _val;
        public Type ValueType => _val.GetType();
        public bool ValueIsArray => ValueType.IsArray;
        public bool ValueIsOneItemArray => ValueIsArray && ValueCount == 1;
        public long ValueCount => !ValueIsArray ? 1 : ((IEnumerable)_val).Cast<object>().ToArray().LongLength;

        public MGAbstractAttribute(object val) => 
            _val = val;
    }
}
