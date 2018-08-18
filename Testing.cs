using MG.Attributes;
using MG.Attributes.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MG
{
    public class Testing : MGNameResolver
    {
        public Testing() { }
    }

    public enum Greetings : int
    {
        Hi = 0,
        Hiya = 1,
        Hey = 2,
        Yo = 3,
        Whatup = 4
    }

    public enum Farewells : int
    {
        [Corresponding("Hi")]
        Bye = 0,

        [Corresponding("Hiya")]
        Byeya = 1,

        [Corresponding("Hey")]
        Seeya = 2,

        [Corresponding("Yo")]
        Peace = 3,

        [Corresponding("Whatup")]
        Later = 4
    }

    public class CorrespondingAttribute : Attribute, IAttribute
    {
        private readonly string _val;
        public object Value => _val;
        public CorrespondingAttribute(string val)
        {
            _val = val;
        }
    }
}
