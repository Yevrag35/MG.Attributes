using MG.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AttributeTesting
{
    public class TestClass
    {
        private AttributeValuator _valuator { get; } = new AttributeValuator();

        [SecondValue("HiddenMessage")]
        public string JustATest { get; } = "LookAtMe";

        public IEnumerable<int> GetByEnum(MyEnum e)
        {
            return _valuator.GetAttributeValues<int, SecondValueAttribute>(e);
        }
        public MyEnum? Get(params int[] nums)
        {
            if (nums == null || nums.Length <= 0)
                return null;

            return _valuator.GetEnumFromValues<int, MyEnum, SecondValueAttribute>(nums);
        }
        public string GetHiddenMessage()
        {
            return _valuator.GetAttributeValue<string, SecondValueAttribute, TestClass, string>(x => x.JustATest);
        }
    }

    public enum MyEnum
    {
        [SecondValue(new int[2] { 23, 43 })]
        Numbers
    }
}
