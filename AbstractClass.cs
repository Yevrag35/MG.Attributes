using MG.Attributes;
using MG.Attributes.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG
{
    public abstract class MGNameResolver
    {

        internal protected T[] GetAttributes<T>(FieldInfo fi, bool failIfMultipleFound = true)
        {
            T[] atts = (fi.GetCustomAttributes(typeof(T), false)) as T[];
            if (atts.Length != 0 || (atts.Length > 1 && !failIfMultipleFound))
            {
                return atts;
            }
            else
            {
                throw new ArgumentException(atts.Length + " attributes matching the type '" + typeof(T).FullName + "' were found!");
            }
        }

        internal protected FieldInfo GetFieldInfo(Enum e) => e.GetType().GetField(e.ToString());
        internal protected Array GetEnumValues(Type t) => t.GetEnumValues();

        // The 'GetName' method should be only used when only the 'MG.Attributes.NameAttribute'
        // attribute on the provided Enum value is present.
        public string GetAttributeName(Enum e)
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
            {
                throw new ArgumentException("The input object's field info could not be retrieved.", x);
            }
            var att = GetAttributes<MGNameAttribute>(fi)[0];
            return att.Name;
        }

        // This 'GetAttributeValue<T>' & 'GetAttributeValues<T>' methods should be used when multiple 'MG.Attributes.Interfaces.IAttribute'
        // attributes on the provided Enum value are present.  Use the 'T' parameter to specify the desired 
        // attribute to retrieve the value for.
        // *NOTE* - 'T', itself, must NOT be an interface, however it must derive from the 'IAttribute' interface.
        public object GetAttributeValue<T>(Enum e)
        {
            if (typeof(T).IsInterface)
            {
                throw new ArgumentException("This generic method does not support interface types!  Use 'GetAttributeValues<T>()!");
            }
            FieldInfo fi = GetFieldInfo(e);
            T[] att = GetAttributes<T>(fi);
            return ((IAttribute)att[0]).Value;
        }


        public object[] GetAttributeValues<T>(Enum e)
        {
            FieldInfo fi = GetFieldInfo(e);
            T[] atts = GetAttributes<T>(fi, false);
            var objs = new List<object>();
            for (int i = 0; i < atts.Length; i++)
            {
                object v = ((IAttribute)atts[i]).Value;
                if (v.GetType().IsArray)
                {
                    var list = (Array)v;
                    for (int i1 = 0; i1 < list.Length; i1++)
                    {
                        object item = list.GetValue(i1);
                        objs.Add(item);
                    }
                }
                else
                {
                    objs.Add(v);
                }
            }
            return objs.ToArray();
        }

        public Enum GetEnumFromValue<T>(Enum nonAttributeEnum, Type attributedEnumType)
        {
            if(typeof(T).IsInterface)
            {
                throw new ArgumentException("This generic method does not support interface types!");
            }
            Array arr = GetEnumValues(attributedEnumType);
            for (int i = 0; i < arr.Length; i++)
            {
                var v = (Enum)arr.GetValue(i);
                object checkO = GetAttributeValue<T>(v);
                if (nonAttributeEnum.ToString().Equals(checkO))
                {
                    return v;
                }
            }
            return null;
        }

        // The 'MatchEnums' method provides a way to match the values of 2 different Enum objects if one of
        // them has their Enums using an 'IAttribute'.  The match will take a normal Enum object and match it
        // to a destination IAttribute's value, returning the designated enum's object.

        public object[] FromNonToAttMatch<T>(Enum nonAttributedEnum, Type typeToReturn)
        {
            if (!typeToReturn.IsEnum)
            {
                throw new ArgumentException(typeToReturn.FullName + " is not a valid enumeration!");
            }
            var retObj = new List<object>();
            string enStr = nonAttributedEnum.ToString();
            Array allVals = GetEnumValues(typeToReturn);
            for (int i = 0; i < allVals.Length; i++)
            {
                var v = (Enum)allVals.GetValue(i);
                object[] attVal = GetAttributeValues<T>(v);
                
                for (int e = 0; e < attVal.Length; e++)
                {
                    object o = attVal[e];
                    if (o.Equals(enStr))
                    {
                        retObj.Add(v);
                    }
                }
                
            }
            return retObj.Count > 0 ? retObj.ToArray() : null;
        }

        public object[] FromAttToNonMatch<T>(Enum attributedEnum, Type enumTypeToMatch)
        {
            if (!enumTypeToMatch.IsEnum)
            {
                throw new ArgumentException(enumTypeToMatch.FullName + " is not a valid enumeration!");
            }
            var retObj = new List<object>();
            Array allPossibleMatches = GetEnumValues(enumTypeToMatch);
            object[] enumAtt = GetAttributeValues<T>(attributedEnum);
            for (int i = 0; i < allPossibleMatches.Length; i++)
            {
                object possible = allPossibleMatches.GetValue(i);
                var s = possible.ToString();
                
                for (int e = 0; e < enumAtt.Length; e++)
                {
                    object o = enumAtt[e];
                    if (o.Equals(s))
                    {
                        retObj.Add(possible);
                    }
                }
            }
            return retObj.Count > 0 ? retObj.ToArray() : null;
        }

    }
}
