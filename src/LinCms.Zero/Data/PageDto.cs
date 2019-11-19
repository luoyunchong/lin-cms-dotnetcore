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
        public int Count { get; set; } = 15;

        /// <summary>
        /// 从0开始，0时取第1页，1时取第二页
        /// </summary>
        public int Page { get; set; } = 0;

        public string Sort { get; set; }
    }

    public interface IPageDto
    {
        int Count { get; set; }
        int Page { get; set; }
    }

}
