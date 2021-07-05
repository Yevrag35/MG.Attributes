using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.Attributes
{
    public interface IValueCollection : IEnumerable
    {
        ICollection GetValueCollection();
        IEnumerable<T> GetValues<T>();
        //IEnumerable<T> GetValuesAs<T>();
    }
}
