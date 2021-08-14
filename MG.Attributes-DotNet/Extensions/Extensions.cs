using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.Attributes
{
    internal static class Extensions
    {
        internal static TProp[] FilterAndReturn<TClass, TProp>(this List<TClass> list, Predicate<TClass> predicate, 
            Func<TClass, TProp> getPropertyOrField)
        {
            IEnumerable<TClass> found = list?.FindAll(predicate);
            return found?.Select(x => getPropertyOrField(x)).Distinct().ToArray();
        }

        internal static bool Overlaps<T>(this T[] array, IEnumerable<T> other)
        {
            var set = new HashSet<T>(array);
            bool result = set.Overlaps(other);
            set.Clear();

            return result;
        }

        internal static TProp[] ToArray<TClass, TProp>(this List<TClass> list, Func<TClass, TProp> getPropertyOrField)
        {
            return GetSubArray((IReadOnlyList<TClass>)list, getPropertyOrField);
        }

        internal static TProp[] ToArray<TClass, TProp>(this IList<TClass> list, Func<TClass, TProp> getPropertyOrField)
        {
            return GetSubArray(list, getPropertyOrField);
        }

        internal static TProp[] ToArray<TClass, TProp>(this IReadOnlyList<TClass> list, Func<TClass, TProp> getPropertyOrField)
        {
            return GetSubArray(list, getPropertyOrField);
        }

        private static TProp[] GetSubArray<TClass, TProp>(IReadOnlyList<TClass> list, Func<TClass, TProp> getPropertyOrField)
        {
            var output = new TProp[(list?.Count).GetValueOrDefault()];
            if (list?.Count <= 0)
                return output;

            for (int i = 0; i < list.Count; i++)
            {
                output[i] = getPropertyOrField(list[i]);
            }

            return output;
        }

        private static TProp[] GetSubArray<TClass, TProp>(IList<TClass> list, Func<TClass, TProp> getPropertyOrField)
        {
            var output = new TProp[(list?.Count).GetValueOrDefault()];
            if (list?.Count <= 0)
                return output;

            for (int i = 0; i < list.Count; i++)
            {
                output[i] = getPropertyOrField(list[i]);
            }

            return output;
        }
    }
}
