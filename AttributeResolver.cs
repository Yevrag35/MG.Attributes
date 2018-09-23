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

            object castedObj = InvokeGenericGetAtts(e, attributeType);
            return (T)((IAttribute[])castedObj)[0].Value;
        }

        public T[] GetAttributeValues<T>(Enum e, Type attributeType)
        {
            FieldInfo fi = GetFieldInfo(e);
            object co = InvokeGenericGetAtts(e, attributeType);
            var ias = (IAttribute[])co;
            var tArr = new T[ias.Length];
            for (int i = 0; i < ias.Length; i++)
            {
                var iat = ias[i];
                tArr[i] = (T)iat.Value;
            }
            return tArr;
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
        public T GetAttEnumByMatchingEnumAttribute<T>(Enum nonAttributedEnum, Type matchingAttributeType) where T : Enum
        {
            var enumString = nonAttributedEnum.ToString();
            T[] tArr = GetEnumValues<T>();
            for (int i = 0; i < tArr.Length; i++)
            {
                T t = tArr[i];
                object val = GetAttributeValue<object>(t, matchingAttributeType);
                if (val.Equals(enumString))
                {
                    return t;
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
                object[] val = GetAttributeValues<object>(t, matchingAttributeType);
                if (val.Contains(enumString))
                {
                    return t;
                }
            }
            return default;
        }

        public T GetNonAttEnumFromAttEnum<T>(Enum attributedEnum, Type matchingAttributeType) where T : Enum
        {
            object val = GetAttributeValue<object>(attributedEnum, matchingAttributeType);
            T[] tArr = GetEnumValues<T>();
            for (int i = 0; i < tArr.Length; i++)
            {
                T t = tArr[i];
                var tStr = t.ToString();
                if (tStr.Equals(val))
                {
                    return t;
                }
            }
            return default;
        }
        public T GetNonAttEnumFromAttEnums<T>(Enum attributedEnum, Type matchingAttributeType) where T : Enum
        {
            object[] val = GetAttributeValues<object>(attributedEnum, matchingAttributeType);
            T[] tArr = GetEnumValues<T>();
            for (int i = 0; i < tArr.Length; i++)
            {
                T t = tArr[i];
                var tStr = t.ToString();
                if (val.Contains(tStr))
                {
                    return t;
                }
            }
            return default;
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

        private protected object InvokeGenericGetAtts(Enum e, Type attType)
        {
            FieldInfo fi = GetFieldInfo(e);
            MethodInfo mi = GetType().GetMethod(
                "GetAttributes", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(attType);
            return mi.Invoke(this, new object[2] { fi, false });
        }

        #endregion
    }
}
