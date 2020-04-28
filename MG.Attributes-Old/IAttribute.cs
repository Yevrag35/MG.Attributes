using System;

namespace MG.Attributes
{
    public interface IAttribute
    {
        Type ValueType { get; }
        object Value { get; }
        bool ValueIsArray { get; }
        bool ValueIsOneItemArray { get; }
        long ValueCount { get; }
    }
}
