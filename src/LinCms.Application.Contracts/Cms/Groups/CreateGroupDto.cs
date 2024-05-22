using LinCms.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinCms.Cms.Groups;

public class CreateGroupDto : UpdateGroupDto, IValidatableObject
{
    public List<long> PermissionIds { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PermissionIds.Count == 0)
        {
            yield return new ValidationResult("请选择权限", new List<string> { "PermissionIds" });
        }
    }
}

public class GroupQuery:PageDto
{
    public string Name { get; set; }

    /// <summary>
    /// 权限组描述
    /// </summary>
    public string Info { get; set; }
}