using System;

namespace LinCms.Domain.Captcha;

public class LoginCaptchaBO
{
    public LoginCaptchaBO()
    {
    }

    public LoginCaptchaBO(string captcha, long expired)
    {
        Captcha = captcha ?? throw new ArgumentNullException(nameof(captcha));
        Expired = expired;
    }

    public string Captcha { get; set; }
    public long Expired { get; set; }
}
