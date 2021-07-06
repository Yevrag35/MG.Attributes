using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;

namespace MG.Attributes.Tests
{
    public class AttributeTest
    {
        private static readonly string[] _testStrings1 = new string[2]
        {
            "one", "two"
        };
        private static readonly string[] _testStrings2 = new string[2]
        {
            "three", "four"
        };

        [Fact]
        public void GetFirstAttributeValue()
        {
            var eval = new AttributeValuator();

            int number = eval.GetAttributeValue<int, AdditionalValueAttribute>(Greetings.Hi);
            Assert.Equal(123, number);
        }

        [Fact]
        public void GetAttributeValues()
        {
            var eval = new AttributeValuator();

            string[] strings = eval.GetAttributeValue<string[], AdditionalValueAttribute>(Greetings.Hello);
            Assert.Equal(_testStrings1, strings);

            string[] maybe = eval.GetAttributeValues<AdditionalValueAttribute, string>(Greetings.Hello);
            Assert.Equal(_testStrings1.Length, maybe.Length);
            for (int i = 0; i < _testStrings1.Length; i++)
            {
                Assert.Equal(_testStrings1[i], maybe[i]);
            }
        }

        [Fact]
        public void GetComplexAttributeValues()
        {
            var eval = new AttributeValuator();

            string[] strings = eval.GetAttributeValues<AdditionalValueAttribute, string>(Greetings.GoodMorning);

            Assert.Equal(_testStrings1.Length, strings.Length);
            Assert.Equal(_testStrings1, strings);
        }

        [Fact]
        public void GetMultipleArrays()
        {
            var eval = new AttributeValuator();
            string[] staticArray = Enumerable.Concat(_testStrings1, _testStrings2).ToArray();

            string[] strings = eval.GetAttributeValues<AdditionalValueAttribute, string>(Greetings.GoodAfternoon);

            Assert.Equal(staticArray.Length, strings.Length);
            for (int i = 0; i < staticArray.Length; i++)
            {
                Assert.Equal(staticArray[i], strings[i]);
            }
        }

        [Fact]
        public void TryGetAttributeValue()
        {
            var eval = new AttributeValuator();

            bool result = eval.TryGetAttributeValue<int, AdditionalValueAttribute>(Greetings.Hi, out int number, out Exception caughtException);
            Assert.True(result);
            Assert.Equal(123, number);
            Assert.Null(caughtException);

            bool result2 = eval.TryGetAttributeValue<string, AdditionalValueAttribute>(Greetings.Hello, out string another, out Exception caughtYa);
            Assert.False(result2);
            Assert.Null(another);
            Assert.NotEqual(123.ToString(), another);
            Assert.NotNull(caughtYa);
            Assert.IsType<InvalidCastException>(caughtYa);
        }

        
    }
}
