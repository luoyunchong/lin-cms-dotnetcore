using LinCms.Entities;

namespace LinCms.Cms.Permissions;

public class PermissioCreateUpdateDto
{
    
    public PermissionType PermissionType { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 所属权限、权限名称，例如：访问首页
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 后台路由
    /// </summary>
    public string Router { get; set; }

    /// <summary>
    ///  排序
    /// </summary>
    public int SortCode { get; set; }
}