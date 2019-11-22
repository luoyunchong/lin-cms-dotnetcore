using System.ComponentModel.DataAnnotations;
using LinCms.Zero.Data.Enums;

namespace LinCms.Web.Models.v1.Tags
{
    public class CreateUpdateTagDto
    {
        [Required(ErrorMessage = "请上传标签封面")]
        public string Thumbnail { get; set; }
        [Required(ErrorMessage = "标签为必填项")]
        public string TagName { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [MaxLength(300, ErrorMessage = "请少于300个字符")]
        public string Alias { get; set; }

        public Status Status { get; set; }

    }
}
