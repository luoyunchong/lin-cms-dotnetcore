using FreeSql.DataAnnotations;

namespace LinCms.Web.Domain
{
    [Table(Name = "lin_auth")]
    public class LinAuth : Entity
    {   /// <summary>
        /// 权限字段
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// 所属权限组
        /// </summary>
        [Column(DbType = "varchar(60)")]
        public string Auth { get; set; }
        /// <summary>
        /// 权限的模块
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Module { get; set; }
    }
}
