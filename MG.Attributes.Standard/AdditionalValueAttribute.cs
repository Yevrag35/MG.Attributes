using System;

namespace MG.Attributes
{
    /// <summary>
    /// An <see cref="Attribute"/> class designed for storing a secondary value of member
    /// (field, property, or <see cref="Enum"/>).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Property, AllowMultiple = true)]
    public class AdditionalValueAttribute : Attribute, IValueAttribute
    {
        public object Value { get; }

        public AdditionalValueAttribute(object value) => this.Value = value;

        public T GetAs<T>() => (T)this.Value;
        public bool ValueIsString() => this.Value is string;
    }
}
