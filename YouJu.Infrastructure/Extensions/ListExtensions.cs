using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouJu.Infrastructure
{
    public static class ListExtensions
    {
        public static bool HasItem<T>(this List<T> list)
        {
            if (list is null)
                return false;
            return list.Count > 0;

        }
        public static string JoinAsString(this IEnumerable<string> source, string separator)
        {

            return string.Join(separator, source);
        }




    }
}
