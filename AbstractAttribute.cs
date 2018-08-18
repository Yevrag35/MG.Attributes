using MG.Attributes.Interfaces;
using System;

namespace MG.Attributes
{
    public abstract class MGAbstractAttribute : Attribute, IAttribute
    {
        private readonly object _val;
        public object Value => _val;
        public MGAbstractAttribute(object val) => _val = val;
    }
}
