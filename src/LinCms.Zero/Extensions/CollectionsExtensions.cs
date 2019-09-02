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
            //Page 方法是从1开始
            return source.Page(pageDto.Page+1, pageDto.Count).ToList();
        }

        public static List<TResult> ToPagerList<TEntity,TResult>(this ISelect<TEntity> source, PageDto pageDto, out long count) where TEntity : class
        {
            count = source.Count();

            return source.Page(pageDto.Page + 1, pageDto.Count).ToList<TResult>();
        }
    }
}
