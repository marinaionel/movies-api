using System;
using System.Collections.Generic;

namespace MoviesApi.Core.Extensions
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
                action(item);
        }
    }
}
