using System;

namespace MG.Attributes.Tests
{
    public class TestAttribute : Attribute, IValueAttribute
    {
        public virtual object Value { get; set; }

        public virtual T GetAs<T>() => throw new NotImplementedException();
        public virtual bool ValueIsString() => throw new NotImplementedException();

        public TestAttribute(object value)
            : base()
        {
            this.Value = value;
        }
    }
    public enum Greetings
    {
        Default,

        [AdditionalValue(123)]
        Hi,

        [AdditionalValue(new string[] { "one", "two" })]
        Hello,

        [AdditionalValue("one")]
        [AdditionalValue("two")]
        GoodMorning,

        [AdditionalValue(new string[] { "one", "two" })]
        [AdditionalValue(new string[] { "three", "four" })]
        GoodAfternoon,

        [AdditionalValue("german")]
        GutenTag
    }
}
