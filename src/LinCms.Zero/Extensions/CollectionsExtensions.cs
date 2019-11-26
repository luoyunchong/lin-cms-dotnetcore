using System.Collections.Generic;
using System.Linq;
using FreeSql;
using LinCms.Zero.Data;

namespace LinCms.Zero.Extensions
{
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Page 方法是从1开始
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageDto"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ISelect<T1, T2> ToPager<T1, T2>(this ISelect<T1, T2> source, PageDto pageDto, out long count)
            where T1 : class
            where T2 : class
        {
            return source.Count(out count).Page(pageDto.Page + 1, pageDto.Count);
        }

        public static ISelect<TEntity> ToPager<TEntity>(this ISelect<TEntity> source, PageDto pageDto, out long count) where TEntity : class
        {
            return source.Count(out count).Page(pageDto.Page + 1, pageDto.Count);
        }
        public static List<TEntity> ToPagerList<TEntity>(this ISelect<TEntity> source, PageDto pageDto, out long count) where TEntity : class
        {
            return source.Count(out count).Page(pageDto.Page + 1, pageDto.Count).ToList();
        }

        public static List<TResult> ToPagerList<TEntity, TResult>(this ISelect<TEntity> source, PageDto pageDto, out long count) where TEntity : class
        {
            return source.Count(out count).Page(pageDto.Page + 1, pageDto.Count).ToList<TResult>();
        }

        public static PagedResultDto<TEntity> ToPagedResultDto<TEntity>(this IReadOnlyList<TEntity> list, long count) where TEntity : class
        {
            return new PagedResultDto<TEntity>(list, count);
        }
    }
}
