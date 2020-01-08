﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace LinCms.Application.Contracts.Cms.Account
{
    public class RegisterDto : IValidatableObject
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(10, MinimumLength = 2, ErrorMessage = "昵称长度必须在2~10之间")]
        [Required(ErrorMessage = "昵称不可为空")]
        public string Nickname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "新密码不可为空")]
        [RegularExpression("^[A-Za-z0-9_*&$#@]{6,22}$", ErrorMessage = "密码长度必须在6~22位之间，包含字符、数字和 _")]
        public string Password { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required(ErrorMessage = "邮件不能为空")]
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Email.IsNullOrEmpty())
            {

                string address = null;
                try
                {
                    address = new MailAddress(Email).Address;
                }
                catch
                {
                    // ignored
                }

                if (address.IsNullOrEmpty())
                {
                    yield return new ValidationResult("电子邮箱不符合规范，请输入正确的邮箱", new[] { "Email" });
                }

            }
        }
    }
}
