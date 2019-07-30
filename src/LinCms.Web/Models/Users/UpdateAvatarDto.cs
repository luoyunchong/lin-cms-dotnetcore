using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Ocsp;

namespace LinCms.Web.Models.Users
{
    public class UpdateAvatarDto
    {
        [Required(ErrorMessage = "请输入头像url")]
        public string Avatar { get; set; }
    }
}
