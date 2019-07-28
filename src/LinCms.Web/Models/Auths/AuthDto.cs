using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.Auths
{
    public class AuthDto:IValidatableObject
    {
        public int GroupId { get; set; }
        public List<string> Auths { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (GroupId <= 0)
            {
                yield return new ValidationResult("分组id必须大于0", new List<string>(){ "GroupId" });
            }
            if (Auths.Count == 0)
            {
                yield return new ValidationResult("请输入auths字段", new List<string>() { "Auths" });
            }
        }
    }
}
