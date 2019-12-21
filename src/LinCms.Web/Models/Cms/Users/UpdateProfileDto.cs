using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.Cms.Users
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "请输入昵称")]
        [StringLength(24, ErrorMessage = "昵称应在24个字符内")]
        public string Nickname { get; set; }

        public string Introduction { get; set; }
    }
}
