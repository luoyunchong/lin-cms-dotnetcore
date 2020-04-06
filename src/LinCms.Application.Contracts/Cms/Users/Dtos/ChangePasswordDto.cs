using System.ComponentModel.DataAnnotations;
using LinCms.Application.Contracts.Cms.Admins.Dtos;

namespace LinCms.Application.Contracts.Cms.Users.Dtos
{
    public class ChangePasswordDto: ResetPasswordDto
    {

        [Required(ErrorMessage = "原密码不可为空")]
        public string OldPassword { get; set; }
    }
}
