using System.Collections.Generic;

namespace Assets.GalaxyCommand.Code.Common
{
    public static class CollectionExtensions
    {
        public static HashSet<T> ToHashSet<T>(
            this IEnumerable<T> source,
            IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
    }
}