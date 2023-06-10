namespace LinCms.Domain.Captcha
{
    /// <summary>
    /// 验证码配置
    /// </summary>
    public class CaptchaOption
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
