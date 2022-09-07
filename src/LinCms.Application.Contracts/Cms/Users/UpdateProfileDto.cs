using System.ComponentModel.DataAnnotations;

namespace LinCms.Cms.Users;

public class UpdateProfileDto
{
    [Required(ErrorMessage = "请输入昵称")]
    [StringLength(24, ErrorMessage = "昵称应在24个字符内")]

    public string Nickname { get; set; }
    [StringLength(100, ErrorMessage = "个人介绍应在100个字符内")]

    public string Introduction { get; set; }

    [StringLength(100, ErrorMessage = "博客应在100个字符内")]
    public string BlogAddress { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [StringLength(50, ErrorMessage = "职位应在50个字符内")]
    public string JobTitle { get; set; }

    /// <summary>
    /// 公司
    /// </summary>
    [StringLength(50, ErrorMessage = "公司应在50个字符内")]
    public string Company { get; set; }
}