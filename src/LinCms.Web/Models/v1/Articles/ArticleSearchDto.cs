using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Data;

namespace LinCms.Web.Models.v1.Articles
{
    public class ArticleSearchDto:PageDto
    {
        public int? ClassifyId { get; set; }
        public int? TagId { get; set; }
        public string Title { get; set; }
    }
}
