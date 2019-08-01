using System.ComponentModel.DataAnnotations;
using LinCms.Web.Models.Cms.Admins;

namespace LinCms.Web.Models.Cms.Users
{
    public class ChangePasswordDto: ResetPasswordDto
    {

        [Required(ErrorMessage = "原密码不可为空")]
        public string OldPassword { get; set; }
    }
}
