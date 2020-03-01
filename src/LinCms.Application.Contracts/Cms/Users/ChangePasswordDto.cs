using System.ComponentModel.DataAnnotations;
using LinCms.Application.Contracts.Cms.Admins;

namespace LinCms.Application.Contracts.Cms.Users
{
    public class ChangePasswordDto: ResetPasswordDto
    {

        [Required(ErrorMessage = "原密码不可为空")]
        public string OldPassword { get; set; }
    }
}
