using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Models.Auths;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.Users
{
    public class UserInformation : EntityDto
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        ///  用户默认生成图像，为null、头像url
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 是否为超级管理员 ;  1 -> 普通用户 |  2 -> 超级管理员
        /// </summary>
        public int Admin { get; set; } = 1;
        /// <summary>
        /// 当前用户是否为激活状态，非激活状态默认失去用户权限 ; 1 -> 激活 | 2 -> 非激活
        /// </summary>
        public int Active { get; set; }
        /// <summary>
        /// 用户所属的权限组id
        /// </summary>
        public int GroupId { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime CreateTime { get; set; }

        public List<IDictionary<string,object>> Auths { get; set; }
    }
}
