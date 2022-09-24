using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities
{
    /// <summary>
    /// 分组权限中间表
    /// </summary>
    [Table(Name = "lin_group_permission")]
    public class LinGroupPermission : Entity<long>
    {
        public LinGroupPermission()
        {
        }

        public LinGroupPermission(long groupId, long permissionId)
        {
            GroupId = groupId;
            PermissionId = permissionId;
        }

        public LinGroupPermission(long permissionId)
        {
            PermissionId = permissionId;
        }

        /// <summary>
        /// 分组id
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 权限Id
        /// </summary>
        public long PermissionId { get; set; }


    }
}
