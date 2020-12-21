using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Cms.Account
{
    public class SendEmailCodeInput : IValidatableObject
    {
        [Required(ErrorMessage = "请输入邮件")]
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
