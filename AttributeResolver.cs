using MG.Attributes.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Attributes
{
    public abstract class AttributeResolver
    {
        #region Public Methods
        public string GetNameAttribute(Enum e)
        {
            FieldInfo fi = null;
            Exception x = null;
            try
            {
                fi = GetFieldInfo(e);
            }
            catch (Exception ex)
            {
                x = ex;
            }

            if (fi == null)
                throw new ArgumentException("The input object's field info could not be retrieved.", x);

            var att = GetAttributes<MGNameAttribute>(fi)[0];
            return att.Name;
        }

        // This 'GetAttributeValue<T>' & 'GetAttributeValues<T>' methods should be used when multiple 'MG.Attributes.Interfaces.IAttribute'
        // attributes on the provided Enum value are present.  Use the 'T' parameter to specify the desired 
        // attribute to retrieve the value for.
        // *NOTE* - 'T', itself, must NOT be an interface, however it must derive from the 'IAttribute' interface.
        public T GetAttributeValue<T>(Enum e, Type attributeType)
        {
            if (!attributeType.GetInterfaces().Contains(typeof(IAttribute)))
                throw new ArgumentException("This method only supports attributes who inherit 'IAttribute'.");
            else if (attributeType.IsInterface)
                throw new ArgumentException("This method does not support interfaces for the attributeType!");

            var castedObj = (IAttribute)InvokeGenericGetAtts(e, attributeType);
            if (castedObj.ValueIsArray)
                throw new InvalidOperationException("The casted object has multiple values!");

            else
                return (T)castedObj.Value;

        }

        public T[] GetAttributeValues<T>(Enum e, Type attributeType)
        {
            if (!attributeType.GetInterfaces().Contains(typeof(IAttribute)))
                throw new ArgumentException("This method only supports attributes who inherit 'IAttribute'.");
            else if (attributeType.IsInterface)
                throw new ArgumentException("This method does not support interfaces for the attributeType!");

            var castedObj = (IAttribute)InvokeGenericGetAtts(e, attributeType);
            if (castedObj == null)
            {
                return default;
            }
            else if (castedObj.ValueIsArray)
            {
                object[] objArr = ((IEnumerable)castedObj.Value).Cast<object>().ToArray();
                var tArr = new T[objArr.Length];
                for (int i = 0; i < objArr.Length; i++)
                {
                    tArr[i] = (T)objArr[i];
                }
                return tArr;
            }
            else
            {
                var tArr = new T[1] { (T)castedObj.Value };
                return tArr;
            }
        }

        public T GetEnumFromValue<T>(object value, Type attributeType) where T : Enum
        {
            T[] arr = GetEnumValues<T>();
            for (int i = 0; i < arr.Length; i++)
            {
                var v = (Enum)arr.GetValue(i);
                object o = GetAttributeValue<object>(v, attributeType);
                if (o.Equals(value))
                {
                    return (T)v;
                }
            }
            return default;
        }
        public T GetAttEnumByMatchingEnumAttributes<T>(Enum nonAttributedEnum, Type matchingAttributeType) where T : Enum
        {
            var enumString = nonAttributedEnum.ToString();
            T[] tArr = GetEnumValues<T>();
            for (int i = 0; i < tArr.Length; i++)
            {
                T t = tArr[i];
                object val = GetAttributeValue<object>(t, matchingAttributeType);
                if (val != null)
                {
                    Type valType = val.GetType();
                    if (valType.IsArray)
                    {
                        var valArr = ((IEnumerable)val).Cast<object>().ToArray();
                        for (int o1 = 0; o1 < valArr.Length; o1++)
                        {
                            object o = valArr[o1];
                            if (o.ToString().Equals(enumString))
                            {
                                return t;
                            }
                        }
                    }
                    else
                    {
                        if (val.ToString().Equals(enumString))
                        {
                            return t;
                        }
                    }
                }
            }
            return default;
        }

        public T GetNonAttEnumFromAttEnum<T>(Enum attributedEnum, Type matchingAttributeType) where T : Enum
        {
            object val = GetAttributeValue<object>(attributedEnum, matchingAttributeType);
            T[] tArr = GetEnumValues<T>();

            var valType = val.GetType();
            switch (valType.IsArray)
            {
                case true:
                    object[] valCol = ((IEnumerable)val).Cast<object>().ToArray();
                    for (int i = 0; i < tArr.Length; i++)
                    {
                        T t = tArr[i];
                        for (int v = 0; v < valCol.Length; v++)
                        {
                            object tVal = valCol[v];
                            if (t.ToString().Equals(tVal.ToString()))
                            {
                                return t;
                            }
                        }
                    }
                    break;
                case false:

                    for (int i = 0; i < tArr.Length; i++)
                    {
                        T t = tArr[i];
                        var tStr = t.ToString();
                        if (tStr.Equals(val))
                        {
                            return t;
                        }
                    }
                    break;
            }
            return default;
        }

        public T[] GetNonAttEnumsFromAttEnum<T>(Enum attributedEnum, Type matchingAttributeType) where T: Enum
        {
            object val = GetAttributeValue<object>(attributedEnum, matchingAttributeType);

            var list = new List<T>(GetEnumValues<T>());

            object[] valArr = ((IEnumerable)val).Cast<object>().ToArray();

            if (!valArr[0].GetType().IsEnum)
                throw new ArgumentException(matchingAttributeType.Name + " must be array of Enum values!");

            for (int i1 = list.Count - 1; i1 >= 0; i1--)
            {
                bool checker = false;
                T t = list[i1];

                for (int i2 = 0; i2 < valArr.Length; i2++)
                {
                    object v = valArr[i2];
                    if (v.Equals(t))
                        checker = true;
                }
                if (!checker)
                    list.Remove(t);
            }

            return list.ToArray();
        }

        #endregion

            #region Private/Backend Methods
        internal protected T[] GetAttributes<T>(FieldInfo fi, bool failIfMultipleFound = true) where T : Attribute, IAttribute
        {
            T[] atts = (fi.GetCustomAttributes(typeof(T), false)) as T[];
            if (atts.Length != 0 || (atts.Length > 1 && !failIfMultipleFound))
                return atts;

            else
                throw new ArgumentException(atts.Length + " attributes matching the type '" + typeof(T).FullName + "' were found!");
        }

        internal protected T Cast<T>(object o) => (T)o;

        internal protected object LoopThroughDynamic<T>(T[] collection, object valToCheck)
        {
            foreach (object o in collection)
            {
                if (o.Equals(valToCheck))
                {
                    return o;
                }
            }
            return null;
        }

        private protected T GetAttribute<T>(FieldInfo fi) where T : Attribute, IAttribute =>
            fi.GetCustomAttribute<T>(false);

        private protected FieldInfo GetFieldInfo(Enum e) =>
            e.GetType().GetField(e.ToString());

        private protected T[] GetEnumValues<T>() where T : Enum
        {
            Array arr = typeof(T).GetEnumValues();
            var tArr = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                var val = (T)arr.GetValue(i);
                tArr[i] = val;
            }
            return tArr;
        }

        private protected const string GenericAttsGetMethod = "GetAttribute";
        private protected object InvokeGenericGetAtts(Enum e, Type attType)
        {
            FieldInfo fi = GetFieldInfo(e);
            Type t = GetType();

            MethodInfo mi = t.GetMethod(GenericAttsGetMethod, BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo mgm = mi.MakeGenericMethod(attType);
            object outObj = null;
            try
            {
                outObj = mgm.Invoke(this, new object[1] { fi });
            }
            catch (TargetInvocationException)
            {
            }
            catch (Exception genEx)
            {
                throw new GenericMethodException(attType, GenericAttsGetMethod, e, genEx.InnerException);
            }
            return outObj;
        }

        #endregion
    }
}
