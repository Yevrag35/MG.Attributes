using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.Attributes
{
    public interface IAttributeValueCollection : IEnumerable
    {
        int Count { get; }
        ICollection GetValueCollection();
        T[] GetValues<T>();
        //IEnumerable<T> GetValuesAs<T>();
    }
}
