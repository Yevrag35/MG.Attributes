using System;

namespace MG.Attributes
{
    public interface IAttributeResolver
    {
        string GetNameAttribute(Enum e);
        T[] GetEnumValues<T>() where T : Enum;

        T GetAttributeValue<T>(Enum e, Type attributeType);
        T[] GetAttributeValues<T>(Enum e, Type attributeType);

        T GetEnumFromValue<T>(object value, Type attributeType) where T : Enum;

        T GetAttEnumByMatchingEnumAttributes<T>(Enum nonAttributeEnum, Type matchingAttributeType) where T : Enum;

        T GetNonAttEnumFromAttEnum<T>(Enum attributedEnum, Type matchingAttributeType) where T : Enum;
        T[] GetNonAttEnumsFromAttEnum<T>(Enum attributeEnum, Type matchingAttributeType) where T : Enum;
    }
}
