using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Data
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
     
    }

}
