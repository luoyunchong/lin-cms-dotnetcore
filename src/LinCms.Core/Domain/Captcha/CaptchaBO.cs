using System;

namespace LinCms.Domain.Captcha;

public class CaptchaBO
{
    public CaptchaBO()
    {
    }

    public CaptchaBO(string captcha, long expired)
    {
        Captcha = captcha ?? throw new ArgumentNullException(nameof(captcha));
        Expired = expired;
    }

    public string Captcha { get; set; }
    public long Expired { get; set; }
}
