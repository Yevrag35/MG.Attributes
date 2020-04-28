using MG.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AttributeTesting
{
    public class TestClass
    {
        private AttributeValuator _valuator { get; } = new AttributeValuator();

        public IEnumerable<int> Get(MyEnum e)
        {
            return _valuator.GetAttributeValues<int, SecondValueAttribute>(e);
        }
        public MyEnum? Get(params int[] nums)
        {
            if (nums == null || nums.Length <= 0)
                return null;

            return _valuator.GetEnumFromValues<int, MyEnum, SecondValueAttribute>(nums);
        }
    }

    public enum MyEnum
    {
        [SecondValue(new int[2] { 23, 43 })]
        Numbers
    }
}
