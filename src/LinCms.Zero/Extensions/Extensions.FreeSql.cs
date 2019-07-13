using System.Collections.Generic;
using FreeSql;
using LinCms.Zero.Data;

namespace LinCms.Zero.Extensions
{
    public static class CollectionsExtensions
    {
        public static List<TEntity> ToPagerList<TEntity>(this ISelect<TEntity> source, PageDto pageDto, out long count) where TEntity : class
        {
            count = source.Count();

            return source.Page(pageDto.Page, pageDto.Count).ToList();
        }

    }
}
