using System;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain
{
    [Table(Name = "lin_auth")]
    public class LinAuth : Entity
    {
        public LinAuth(string auth, string module, int groupId)
        {
            Auth = auth ?? throw new ArgumentNullException(nameof(auth));
            Module = module ?? throw new ArgumentNullException(nameof(module));
            GroupId = groupId;
        }

        public LinAuth()
        {
        }

        public LinAuth(string auth, int groupId)
        {
            GroupId = groupId;
            Auth = auth ?? throw new ArgumentNullException(nameof(auth));
        }

        /// <summary>
        /// 权限字段
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// 所属权限
        /// </summary>
        [Column(DbType = "varchar(60)")]
        public string Auth { get; set; }
        /// <summary>
        /// 权限的模块
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Module { get;  set; }

        public override string ToString()
        {
            return base.ToString() + $" Auth:{Auth}、Module:{Module}";
        }
    }

}
