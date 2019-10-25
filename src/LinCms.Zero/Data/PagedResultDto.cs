using System.Collections.Generic;

namespace LinCms.Zero.Data
{
    public class PagedResultDto<T>
    {
        public long Total { get; set; }

        public IReadOnlyList<T> Items
        {
            get => _items ?? new List<T>();
            set => _items = value;
        }
        private IReadOnlyList<T> _items;

        public PagedResultDto(IReadOnlyList<T> items)
        {
            Total = items.Count;
            _items = items;
        }


        public PagedResultDto(IReadOnlyList<T> collection, long total) : this(collection)
        {
            Total = total;
        }
    }
}
