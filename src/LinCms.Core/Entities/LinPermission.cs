using System;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities
{
    /// <summary>
    /// 权限表
    /// </summary>
    [Table(Name = "lin_permission")]
    public class LinPermission : FullAuditEntity<long, long>
    {
        public LinPermission()
        {
        }

        public LinPermission(string name, PermissionType permissionType, string router)
        {
            Name = name;
            PermissionType = permissionType;
            Router = router;
        }

        public PermissionType PermissionType { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 所属权限、权限名称，例如：访问首页
        /// </summary>
        [Column(StringLength = 60)]
        public string Name { get; set; }

        /// <summary>
        /// 后台路由
        /// </summary>
        [Column(StringLength = 200)]
        public string Router { get; set; }

        /// <summary>
        ///  排序
        /// </summary>
        public int SortCode { get; set; }

    }

    public enum PermissionType
    {
        Folder = 0,
        Permission = 1
    }
}
