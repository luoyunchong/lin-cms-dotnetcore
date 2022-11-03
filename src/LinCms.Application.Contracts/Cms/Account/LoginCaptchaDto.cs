using System;

namespace LinCms.Cms.Users
{
    public class LoginCaptchaDto
    {
        public LoginCaptchaDto()
        {
        }

        public LoginCaptchaDto(string tag, string image)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        /// <summary>
        /// 验证码图片地址，可使用base64
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 加密后的验证码
        /// </summary>
        public string Image { get; set; }
    }
}
