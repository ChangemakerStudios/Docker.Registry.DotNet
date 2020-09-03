using System.Collections.Generic;
using System.Linq;

namespace Docker.Registry.DotNet.Helpers
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> IfNullEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }
    }
}