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
            Greetings y = eval.GetEnumFromValue<Greetings, AdditionalValueAttribute>("german");
            Assert.Equal(Greetings.Hi, e);
            Assert.Equal(Greetings.GutenTag, y);
        }

        [Fact]
        public void GetEnumFromValues()
        {
            var eval = new AttributeValuator();

            Greetings output = eval.GetEnumFromValues<Greetings, AdditionalValueAttribute, string>(new string[]
            {
                "one", "two"
            });

            Assert.Equal(Greetings.Hello, output);
        }

        [Fact]
        public void GetEnumsFromValue()
        {
            var eval = new AttributeValuator();

            Greetings[] output = eval.GetEnumsFromValue<Greetings, AdditionalValueAttribute>(123);
            Assert.Equal(2, output.Length);
            Assert.True(output.All(x => x == Greetings.Hi || x == Greetings.Yo));
        }

        [Fact]
        public void GetEnumsFromValues()
        {
            var eval = new AttributeValuator();

            Greetings[] output = eval.GetEnumsFromValues<Greetings, AdditionalValueAttribute, string>(new string[] { "one", "two", "german" });
            Assert.Equal(4, output.Length);
            Assert.True(output.All(x => x == Greetings.GutenTag || x == Greetings.Hello || x == Greetings.GoodAfternoon || x == Greetings.GoodMorning));
        }
    }
}
