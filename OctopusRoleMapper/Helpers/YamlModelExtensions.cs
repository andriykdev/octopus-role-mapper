using System;
using System.Linq;

namespace OctopusRoleMapper.Helpers
{
    internal static class YamlModelExtensions
    {
        public static TProperty[] MergeItemsIn<TModel, TProperty>(this TModel dst, TModel src, Func<TModel, TProperty[]> itemsSelector)
        {
            return itemsSelector(dst).EnsureNotNull().Concat(itemsSelector(src).EnsureNotNull()).ToArray().NullIfEmpty();
        }
    }
}
