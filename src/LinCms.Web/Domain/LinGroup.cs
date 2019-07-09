
using FreeSql.DataAnnotations;

namespace LinCms.Web.Domain
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

    }
}
