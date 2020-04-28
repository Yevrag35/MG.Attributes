using System;
using System.Reflection;

namespace MG.Attributes.Exceptions
{
    public class GenericMethodException : TargetException
    {
        private protected const string defMsg = "{0} threw an exception when searching for attribute values!";
        private Type _attType;
        private string _mn;
        private Enum _e;

        public Type AttributeType => _attType;
        public string OffendingMethod => _mn;
        public Enum OffendingEnum => _e;

        public GenericMethodException(Type attType, string methodName, Enum e)
            : base(string.Format(defMsg, methodName))
        {
            _attType = attType;
            _mn = methodName;
            _e = e;
        }

        public GenericMethodException(Type attType, string methodName, Enum e, Exception innerException)
            : base(string.Format(defMsg, methodName), innerException)
        {
            _attType = attType;
            _mn = methodName;
            _e = e;
        }
    }
}
