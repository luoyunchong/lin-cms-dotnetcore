using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Cms.Users.Dtos
{
    public class UpdateAvatarDto
    {
        [Required(ErrorMessage = "请输入头像url")]
        public string Avatar { get; set; }
    }
}
