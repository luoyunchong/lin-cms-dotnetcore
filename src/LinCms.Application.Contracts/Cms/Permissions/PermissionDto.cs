using System;
using System.Collections.Generic;
using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Entities;

namespace LinCms.Cms.Permissions;

public class PermissionDto : EntityDto<long>
{
    /// <summary>
    /// 父级Id
    /// </summary>
    public long ParentId { get; set; }
    public string Name { get; set; }

    public PermissionType PermissionType { get; set; }

    public string Router { get; set; }

}

public class PermissionTreeNode : TreeNode
{
    public int SortCode { get; set; }
    public string Router { get; set; }
    public PermissionType PermissionType { get; set; }
    public DateTime? CreateTime { get; set; }
    public List<PermissionTreeNode> Children { get; set; }

    public PermissionTreeNode()
    {
        Children = new List<PermissionTreeNode>();
    }

}

public class TreeNode
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public string Name { get; set; }
    public List<TreeNode> Children { get; set; }

    public TreeNode()
    {
        Children = new List<TreeNode>();
    }
}