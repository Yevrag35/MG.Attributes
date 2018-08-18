using MG.Attributes.Interfaces;
using System;

namespace MG.Attributes
{
    public class MGNameAttribute : MGAbstractAttribute
    {
        public string Name => (string)Value;

        public MGNameAttribute(string name)
            : base(name)
        {
        }
    }
}
