using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.Users
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "")]
        public string OldPassword { get; set; }
    }
}
