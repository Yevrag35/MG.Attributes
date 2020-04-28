using MG.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AttributeTesting
{
    public class TestClass
    {
        private AttributeValuator _valuator { get; } = new AttributeValuator();

        [AdditionalValue("HiddenMessage")]
        [AnotherSecond("AnotherHiddenMessage")]
        public string JustATest { get; } = "LookAtMe";

        public IEnumerable<int> GetByEnum(MyEnum e)
        {
            return _valuator.GetAttributeValues<int, AdditionalValueAttribute>(e);
        }
        public MyEnum? Get(params int[] nums)
        {
            if (nums == null || nums.Length <= 0)
                return null;

            return _valuator.GetEnumFromValues<int, MyEnum, AdditionalValueAttribute>(nums);
        }
    }

    public enum MyEnum
    {
        [AdditionalValue(new int[2] { 23, 43 })]
        [AnotherSecond(new int[3] { 1, 2, 3 })]
        Numbers
    }

    public class AnotherSecondAttribute : AdditionalValueAttribute
    {
        public AnotherSecondAttribute(object val) : base(val) { }
    }
}
