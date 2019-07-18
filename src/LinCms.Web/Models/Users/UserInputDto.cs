using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.Users
{
    public class UserInputDto:IValidatableObject
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [MinLength(length:2,ErrorMessage = "昵称长度必须在2~10之间")]
        [MaxLength(length:10,ErrorMessage = "昵称长度必须在2~10之间")]
        [Required(ErrorMessage = "昵称不可为空")]
        public string Nickname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "新密码不可为空")]
        [Compare("ConfirmPassword",ErrorMessage = "两次输入的密码不一致，请输入相同的密码")]
        [RegularExpression("^[A-Za-z0-9_*&$#@]{6,22}$")]
        public string Password { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "请确认密码")]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [EmailAddress(ErrorMessage = "电子邮箱不符合规范，请输入正确的邮箱")]
        public string Email { get; set; }
        /// <summary>
        /// 用户所属的权限组id
        /// </summary>
        [Required(ErrorMessage = "请输入分组id")]
        [MinLength(1,ErrorMessage = "分组id必须大于0")]
        public int? GroupId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return new ValidationResult("分组不存在", new[] { "GroupId" });
        }
    }
}
