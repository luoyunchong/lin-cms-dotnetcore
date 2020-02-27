using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Cms.Permissions
{
    public class PermissionDto:IValidatableObject
    {
        public long GroupId { get; set; }
        public List<long> Permission { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (GroupId <= 0)
            {
                yield return new ValidationResult("分组id必须大于0", new List<string>(){ "GroupId" });
            }
            if (Permission.Count == 0)
            {
                yield return new ValidationResult("请输入Permission字段", new List<string>() { "Permission" });
            }
        }
    }
}
