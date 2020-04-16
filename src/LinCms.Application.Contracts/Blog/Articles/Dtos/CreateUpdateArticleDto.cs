using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.Articles.Dtos
{
    public class CreateUpdateArticleDto
    {
        public Guid? ClassifyId { get; set; }
        public Guid? ChannelId { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(400)]
        public string Keywords { get; set; }
        [MaxLength(400)]
        public string Source { get; set; }
        [MaxLength(400)]
        public string Excerpt { get; set; }
        [Required(ErrorMessage = "文章内容不能为空")]
        public string Content { get; set; }
        [MaxLength(400)]
        public string Thumbnail { get; set; }
        public bool IsAudit { get; set; }
        public bool Recommend { get; set; }
        public bool IsStickie { get; set; }
        [MaxLength(50)]
        public string Archive { get; set; }

        public ArticleType ArticleType { get; set; }

        public int Editor { get; set; } = 1;

        public List<Guid> TagIds { get; set; }
    }

}
