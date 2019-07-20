using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Models.Admins;

namespace LinCms.Web.Models.Users
{
    public class ChangePasswordDto: ResetPasswordDto
    {

        [Required(ErrorMessage = "原密码不可为空")]
        public string OldPassword { get; set; }
    }
}
