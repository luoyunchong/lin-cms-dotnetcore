
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    [Table(Name = "lin_group")]
    public class LinGroup : Entity
    {
        /// <summary>
        /// 权限组名称
        /// </summary>
        [Column(DbType = "varchar(60)")]
        public string Name { get; set; }
        /// <summary>
        /// 权限组描述
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 是否是静态角色
        /// </summary>
        public bool IsStatic { get; set; } = true;
        /// <summary>
        /// 超级管理员
        /// </summary>
        public const string Admin = "Admin";
        /// <summary>
        /// Cms管理员
        /// </summary>
        public const string CmsAdmin = "CmsAdmin";

        /// <summary>
        /// 普通用户
        /// </summary>
        public const string User = "User";

    }
}
