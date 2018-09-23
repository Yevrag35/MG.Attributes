using System;

namespace MG.Attributes
{
    public abstract class MGAbstractAttribute : Attribute, IAttribute
    {
        private readonly object _val;
        public object Value => _val;
        public Type ValueType => _val.GetType();
        public bool ValueIsArray => ValueType.IsArray;

        public MGAbstractAttribute(object val) => 
            _val = val;
    }
}
