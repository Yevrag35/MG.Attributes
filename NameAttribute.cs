using MG.Attributes.Interfaces;
using System;

namespace MG.Attributes
{
    public class MGNameAttribute : Attribute, IAttribute
    {
        private string _name;
        public object Value { get { return _name; } }
        public MGNameAttribute(string name)
        {
            _name = name;
        }
    }
}
