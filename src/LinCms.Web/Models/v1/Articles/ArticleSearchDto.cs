using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Data;

namespace LinCms.Web.Models.v1.Articles
{
    public class ArticleSearchDto:PageDto
    {
        public Guid? ClassifyId { get; set; }
        public Guid? TagId { get; set; }
        public string Title { get; set; }
    }
}
