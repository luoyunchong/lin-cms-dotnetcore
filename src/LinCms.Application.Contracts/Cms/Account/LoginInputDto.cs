using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Cms.Account;

public class LoginInputDto
{
    /// <summary>
    /// 登录名:admin
    /// </summary>
    [Required(ErrorMessage = "登录名为必填项")]
    [DefaultValue("admin")]
    public string Username { get; set; }
    /// <summary>
    /// 密码：123qwe
    /// </summary>
    [Required(ErrorMessage = "密码为必填项")]
    [DefaultValue("123qwe")]
    public string Password { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    [CanBeNull]
    public string Captcha { get; set; }

    [CanBeNull] 
    public string Tag { get; set; }
}