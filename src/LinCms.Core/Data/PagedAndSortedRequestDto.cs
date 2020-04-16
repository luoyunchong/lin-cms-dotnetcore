namespace LinCms.Core.Data
{

    public class PagedAndSortedRequestDto:PageDto,ISortedResultRequest
    {
        public string Sorting { get; set; }
    }


    /// <summary>
    /// 分页
    /// </summary>
    public class PageDto: IPageDto
    {
        /// <summary>
        /// 每页个数
        /// </summary>
        public int Count { get; set; } = 15;

        /// <summary>
        /// 从0开始，0时取第1页，1时取第二页
        /// </summary>
        public int Page { get; set; } = 0;

        public string Sort { get; set; }
    }

    public interface IPageDto:ILimitedResultRequest
    {
        int Page { get; set; }
    }

    public interface ILimitedResultRequest
    {
        /// <summary>
        /// Maximum result count should be returned.
        /// This is generally used to limit result count on paging.
        /// </summary>
        int Count { get; set; }
    }
    
    /// <summary>
    /// This interface is defined to standardize to request a sorted result.
    /// </summary>
    public interface ISortedResultRequest
    {
        /// <summary>
        /// Sorting information.
        /// Should include sorting field and optionally a direction (ASC or DESC)
        /// Can contain more than one field separated by comma (,).
        /// </summary>
        /// <example>
        /// Examples:
        /// "Name"
        /// "Name DESC"
        /// "Name ASC, Age DESC"
        /// </example>
        string Sorting { get; set; }
    }
}
