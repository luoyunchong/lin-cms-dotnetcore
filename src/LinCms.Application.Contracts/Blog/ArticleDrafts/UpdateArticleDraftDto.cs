using System.ComponentModel.DataAnnotations;

namespace LinCms.Blog.ArticleDrafts;

public class UpdateArticleDraftDto
{
    [MaxLength(200)]
    public string Title { get; set; }

    [Required(ErrorMessage = "随笔内容不能为空")]
    public string Content { get; set; }

}