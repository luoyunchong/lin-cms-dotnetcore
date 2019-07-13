using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain
{
    [Table(Name = "lin_event")]
    public class LinEvent : Entity
    {
        /// <summary>
        /// 所属权限组id
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        [Column(DbType = "varchar(250)")]
        public string MessageEvents { get; set; } = string.Empty;

    }
}
