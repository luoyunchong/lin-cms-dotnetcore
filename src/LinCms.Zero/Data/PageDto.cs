namespace LinCms.Zero.Data
{

    /// <summary>
    /// 分页
    /// </summary>
    public class PageDto: IPageDto
    {
        /// <summary>
        /// 每页个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 第几页
        /// </summary>
        public int Page { get; set; }
    }

    public interface IPageDto
    {
        int Count { get; set; }
        int Page { get; set; }
    }

}
