using System.Collections.Generic;
using FreeSql;
using LinCms.Core.Data;

namespace LinCms.Core.Extensions
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

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue obj;
            return dictionary.TryGetValue(key, out obj) ? obj : default;
        }
    }
}
