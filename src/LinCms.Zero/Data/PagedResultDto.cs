using System.Collections.Generic;

namespace LinCms.Zero.Data
{
    public class PagedResultDto<T>
    {
        public long TotalCount { get; set; }

        public IReadOnlyList<T> Items
        {
            get => _items ?? new List<T>();
            set => _items = value;
        }
        private IReadOnlyList<T> _items;
        public PagedResultDto()
        {

        }
        public PagedResultDto(IReadOnlyList<T> items)
        {
            TotalCount = items.Count;
            Items = items;
        }


        public PagedResultDto(long totalCount, IReadOnlyList<T> items) : this(items)
        {
            TotalCount = totalCount;
        }
    }
}
