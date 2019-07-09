using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Data
{
    public class PagedResultDto<T>
    {
        public long TotalCount { get; set; }

        public IReadOnlyList<T> Items
        {
            get => _items ?? (_items = new List<T>());
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
