using MG.Attributes;
using MG.Attributes.Interfaces;
using System;
using System.Reflection;

namespace MG
{
    public abstract class MGNameResolver
    {
        // The 'GetName' method should be only used when only the 'MG.Attributes.NameAttribute'
        // attribute on the provided Enum value is present.
        public string GetName(Enum e)
        {
            FieldInfo fi = e.GetType().GetField(e.ToString());
            MGNameAttribute att = ((fi.GetCustomAttributes(typeof(MGNameAttribute), false)) as MGNameAttribute[])[0];
            return (string)att.Value;
        }

        // This 'GetValue' method overload should be used when only 1 'MG.Attributes.Interfaces.IAttribute'
        // attribute on the provided Enum value is present.
        public object GetValue(Enum e)
        {
            FieldInfo fi = e.GetType().GetField(e.ToString());
            IAttribute att = ((fi.GetCustomAttributes(typeof(IAttribute), false)) as IAttribute[])[0];
            return att.Value;
        }

        // This 'GetValue' method overload should be used when multiple 'MG.Attributes.Interfaces.IAttribute'
        // attributes on the provided Enum value are present.  Use the 't' parameter to specify the desired 
        // attribute to retrieve the value for.
        public object GetValue(Enum e, Type t)
        {
            FieldInfo fi = e.GetType().GetField(e.ToString());
            IAttribute att = ((fi.GetCustomAttributes(t, false)) as IAttribute[])[0];
            return att.Value;
        }
    }
}
