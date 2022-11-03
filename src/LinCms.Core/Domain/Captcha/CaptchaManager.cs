using System;
using System.Text;
using DotNetCore.Security;
using IGeekFan.FreeKit.Extras.Dependency;
using LinCms.Common;
using LinCms.Data.Options;
using Newtonsoft.Json;

namespace LinCms.Domain.Captcha;

public class CaptchaManager : ICaptchaManager, ITransientDependency
{
    /// <summary>
    /// 验证码字符个数
    /// </summary>
    public static int RandomStrNum = 4;
    public static string Salt = "cryptography_salt";
    private static readonly string RandomString = "23456789abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWSYZ";
    private readonly ICryptographyService _cryptographyService;
    public CaptchaManager(ICryptographyService cryptographyService)
    {
        _cryptographyService = cryptographyService;
    }

    public string GetTag(string captcha, string salt = "cryptography_salt", int seconds = 300)
    {
        if (string.IsNullOrWhiteSpace((salt))) salt = Salt;
        LoginCaptchaBO captchaBo = new(captcha, GetTimeStamp(seconds));
        var json = JsonConvert.SerializeObject(captchaBo);
        return _cryptographyService.Encrypt(json, salt);
    }
    /// <summary>  
    /// 获取时间戳  13位
    /// </summary>  
    /// <returns></returns>  
    public long GetTimeStamp(int seconds = 0)
    {
        TimeSpan ts = DateTime.UtcNow.AddSeconds(seconds) - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds * 1000);
    }
    /// <summary>
    /// 随机字符的获取
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public string GetRandomString(int num)
    {
        Random random = new();
        num = num > 0 ? num : CaptchaManager.RandomStrNum;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < num; i++)
        {
            int number = random.Next(RandomString.Length);
            sb.Append(RandomString[number]);
        }
        return sb.ToString();
    }

    public string GetRandomCaptchaBase64(string captcha)
    {
        var bytes = ImgHelper.GetVerifyCode(captcha);
        return Convert.ToBase64String(bytes);
    }

    public LoginCaptchaBO DecodeTag(string tag, string salt = "cryptography_salt")
    {
        if (string.IsNullOrWhiteSpace((salt))) salt = Salt;
        string json = _cryptographyService.Decrypt(tag, salt);
        var loginCaptchaBo = JsonConvert.DeserializeObject<LoginCaptchaBO>(json);
        return loginCaptchaBo;
    }

}
