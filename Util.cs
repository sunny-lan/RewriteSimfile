using System;
using System.Collections.Generic;
using System.Text;

namespace OsuSM
{
    delegate bool Predicate<T>(T x);
    static class Util
    {
        /// <summary>
        /// Finds the first element x for which P(x) is true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="p">A function which is true for all s[i] where i>y. Y is the return value</param>
        /// <returns>The index of the element, s.Count if no element found</returns>
        public static int LowerBound<T>(this List<T> s, Predicate<T> p)
        {
            int lo = 0, hi = s.Count;
            while (lo != hi)
            {
                int mid = (lo + hi) / 2;
                if (!p(s[mid]))
                    lo = mid + 1;
                else
                    hi = mid;
            }
            return lo;
        }

        public static void Shuffle<T>(this T[] array, Random random)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int i = random.Next(n + 1);
                T temp = array[i];
                array[i] = array[n];
                array[n] = temp;
            }
        }
    }
}
