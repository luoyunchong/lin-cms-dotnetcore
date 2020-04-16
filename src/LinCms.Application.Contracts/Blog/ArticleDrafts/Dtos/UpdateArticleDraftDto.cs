using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Blog.ArticleDrafts.Dtos
{
    public class UpdateArticleDraftDto
    {
        [MaxLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "文章内容不能为空")]
        public string Content { get; set; }

    }
}
