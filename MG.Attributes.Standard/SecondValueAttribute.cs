using System;

namespace MG.Attributes
{
    /// <summary>
    /// An <see cref="Attribute"/> class designed for storing a secondary value of member
    /// (field, property, or <see cref="Enum"/>).
    /// </summary>
    public class SecondValueAttribute : Attribute, IValueAttribute
    {
        public object Value { get; }

        public SecondValueAttribute(object value) => this.Value = value;

        public T GetAs<T>() => (T)this.Value;
        public bool ValueIsString() => this.Value is string;
    }
}
