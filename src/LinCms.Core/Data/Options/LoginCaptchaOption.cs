using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Data.Options
{
    /// <summary>
    /// 验证码配置
    /// </summary>
    public class LoginCaptchaOption
    {
        /// <summary>
        /// 是否启用验证码
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 盐值
        /// </summary>
        public string Salt { get; set; }
    }
}
