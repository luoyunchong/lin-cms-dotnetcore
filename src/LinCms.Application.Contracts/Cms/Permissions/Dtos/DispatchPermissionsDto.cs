using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Cms.Permissions.Dtos
{
    public class DispatchPermissionsDto : IValidatableObject
    {
        public long GroupId { get; set; }
        public List<long> PermissionIds { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (GroupId <= 0)
            {
                yield return new ValidationResult("分组id必须大于0", new List<string>(){ "GroupId" });
            }
            if (PermissionIds.Count == 0)
            {
                yield return new ValidationResult("请输入PermissionIds字段", new List<string>() { "Permission" });
            }
        }
    }
}
