using Moq;
using Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.Attributes.Tests
{
    public class EnumTest
    {
        [Fact]
        public void GetEnumFromValue()
        {
            var eval = new AttributeValuator();

            Greetings e = eval.GetEnumFromValue<Greetings, AdditionalValueAttribute>(123);
            Assert.Equal(Greetings.Hi, e);
        }

        [Fact]
        public void GetEnumsFromValues()
        {
            var eval = new AttributeValuator();

            Greetings output = eval.GetEnumFromValues<Greetings, AdditionalValueAttribute, string>(new string[]
            {
                "one", "two"
            });

            Assert.Equal(Greetings.Hello, output);
        }

        [Fact]
        public void GetEnums
    }
}
