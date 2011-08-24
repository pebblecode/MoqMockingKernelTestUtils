using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestUtils
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adds the useful ForEach() function to IEnumerable collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T t in enumerable)
            {
                action(t);
            }
        }
    }
}
