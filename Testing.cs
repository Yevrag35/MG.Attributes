using MG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace MG
{
    public class Testing : AttributeResolver
    {
        public Testing() { }

        public Farewells GetFarewallByString(string str)
        {
            return GetEnumFromValue<Farewells>(str, typeof(CorrespondingAttribute));
        }

        public DriveType GetDriveTypesFromFarewall(Farewells farewell)
        {
            return GetAttributeValue<DriveType>(farewell, typeof(EnumAttribute));
        }

        public Farewells GetFarewallFromGreeting(Greetings greeting)
        {
            return GetAttEnumByMatchingEnumAttributes<Farewells>(greeting, typeof(CorrespondingAttribute));
        }

        public Farewells GetFarewallByDriveType(DriveType dT)
        {
            return GetAttEnumByMatchingEnumAttributes<Farewells>(dT, typeof(EnumAttribute));
        }

        public Greetings GetGreetingByFarewallAttribute(Farewells fare)
        {
            return GetNonAttEnumFromAttEnum<Greetings>(fare, typeof(CorrespondingAttribute));
        }
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

        [Enum(new DriveType[2] { DriveType.Fixed, DriveType.Ram })]
        [Corresponding("Hey")]
        Seeya = 2,

        [Corresponding("Yo")]
        Peace = 3,

        [Corresponding("Whatup")]
        Later = 4
    }

    public class CorrespondingAttribute : MGAbstractAttribute
    {
        public CorrespondingAttribute(string val)
            : base(val)
        {
        }
    }
    public class EnumAttribute : MGAbstractAttribute
    {
        public EnumAttribute(DriveType[] dTs)
            : base(dTs)
        {
        }
    }
}
