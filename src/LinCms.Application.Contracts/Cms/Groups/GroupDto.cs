using System.Collections.Generic;
using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Entities;

namespace LinCms.Cms.Groups;

public class GroupDto : Entity<long>
{
    public List<LinPermission> Permissions { get; set; }
    public string Name { get; set; }
    public string Info { get; set; }
    public bool IsStatic { get; set; }

}