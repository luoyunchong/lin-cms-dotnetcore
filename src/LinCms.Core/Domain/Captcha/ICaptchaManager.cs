namespace LinCms.Domain.Captcha;

public interface ICaptchaManager
{

    /// <summary>
    /// 获取加密后验证码
    /// </summary>
    /// <param name="captcha">原验证码</param>
    /// <param name="salt">盐值</param>
    /// <param name="seconds">过期时间（单位秒）</param>
    /// <returns></returns>
    string GetTag(string captcha, string salt = "cryptography_salt", int seconds = 300);

    /// <summary>
    /// 获取时间戳（13位)
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    long GetTimeStamp(int seconds=0);
    /// <summary>
    /// 随机字符的获取 
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    string GetRandomString(int num);

    /// <summary>
    /// 生成随机图片的base64编码字符串
    /// </summary>
    /// <param name="captcha"></param>
    /// <returns></returns>
    string GetRandomCaptchaBase64(string captcha);

    /// <summary>
    /// 解密出验证码
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    LoginCaptchaBO DecodeTag(string tag, string salt = "cryptography_salt");
}