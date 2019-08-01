using System.ComponentModel.DataAnnotations;

namespace LinCms.Web.Models.Cms.Users
{
    public class UpdateAvatarDto
    {
        [Required(ErrorMessage = "请输入头像url")]
        public string Avatar { get; set; }
    }
}
