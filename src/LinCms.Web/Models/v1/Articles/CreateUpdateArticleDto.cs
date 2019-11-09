using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.Models.v1.Articles
{
    public class CreateUpdateArticleDto
    {
        public Guid Id { get; set; }
        public Guid? ClassifyId { get; set; }
        [StringLength(200)]
        public string Title { get; set; }
        [StringLength(400)]
        public string Keywords { get; set; }
        [StringLength(400)]
        public string Source { get; set; }
        [StringLength(400)]
        public string Excerpt { get; set; }
        public string Content { get; set; }
        [StringLength(400)]
        public string Thumbnail { get; set; }
        public bool IsAudit { get; set; }
        public bool Recommend { get; set; }
        public bool IsStickie { get; set; }
        [StringLength(50)]
        public string Archive { get; set; }

        public ArticleType ArticleType { get; set; }
        public int Editor { get; set; } = 1;

        public List<Guid> TagIds { get; set; }
    }

}
