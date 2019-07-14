using System.Collections.Generic;

namespace LinCms.Zero.Data
{
    public class PagedResultDto<T>
    {
        public long TotalNums { get; set; }

        public IReadOnlyList<T> Collection
        {
            get => _collection ?? new List<T>();
            set => _collection = value;
        }
        private IReadOnlyList<T> _collection;

        public PagedResultDto(IReadOnlyList<T> collection)
        {
            TotalNums = collection.Count;
            _collection = collection;
        }


        public PagedResultDto(IReadOnlyList<T> collection, long totalCount) : this(collection)
        {
            TotalNums = totalCount;
        }
    }
}
