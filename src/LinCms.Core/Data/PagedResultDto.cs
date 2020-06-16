using System.Collections.Generic;

namespace LinCms.Core.Data
{
    public class PagedResultDto<T>
    {
        public long Total { get; set; }
        public IReadOnlyList<T> Items { get; set; }
        public long Page { get; set; }
        public long Count { get; set; }

        public PagedResultDto()
        {
        }
        public PagedResultDto(IReadOnlyList<T> items)
        {
            Total = items.Count;
            Items = items;
        }
        public PagedResultDto(IReadOnlyList<T> items, long total) : this(items)
        {
            Total = total;
        }
    }
}
