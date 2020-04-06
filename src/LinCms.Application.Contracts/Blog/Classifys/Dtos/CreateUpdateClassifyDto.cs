using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Blog.Classifys.Dtos
{
    public class CreateUpdateClassifyDto
    {
        [Required(ErrorMessage = "请上传专栏图")]
        public string Thumbnail { get; set; }
        public int SortCode { get; set; }
        [Required(ErrorMessage = "分类专栏为必填项")]
        public string ClassifyName { get; set; }
    }
}
