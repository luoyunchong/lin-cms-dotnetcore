using System.ComponentModel.DataAnnotations;

namespace LinCms.Cms.Groups
{
    public class UpdateGroupDto
    {
        [Required(ErrorMessage = "请输入分组名称")]
        public string Name { get; set; }
        public string Info { get; set; }
    }
}
