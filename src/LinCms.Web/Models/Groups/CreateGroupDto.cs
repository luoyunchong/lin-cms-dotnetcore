using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.Groups
{
    public class CreateGroupDto: UpdateGroupDto
    {
        [Required(ErrorMessage = "请输入auths字段")]
        public List<string> Auths { get; set; }
    }
}
