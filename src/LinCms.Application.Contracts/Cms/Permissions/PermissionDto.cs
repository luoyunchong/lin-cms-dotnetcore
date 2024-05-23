using System;
using System.Collections.Generic;
using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Entities;

namespace LinCms.Cms.Permissions;

public class PermissionDto : EntityDto<long>
{
    public string Name { get; set; }

    public PermissionType PermissionType { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public long ParentId { get; set; }

    public string Router { get; set; }

}

public class PermissionTreeNode : PermissionNode
{
    public string Router { get; set; }

    public DateTime? CreateTime { get; set; }
    public List<PermissionTreeNode> Children { get; set; }

    public PermissionTreeNode()
    {
        Children = new List<PermissionTreeNode>();
    }

}

public class PermissionNode
{
    public long Id { get; set; }
    public PermissionType PermissionType { get; set; }
    public long ParentId { get; set; }
    public string Name { get; set; }
    public List<PermissionNode> Children { get; set; }

    public PermissionNode()
    {
        Children = new List<PermissionNode>();
    }
}