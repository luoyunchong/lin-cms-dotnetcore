using System;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    [Table(Name = "lin_permission")]
    public class LinPermission : Entity
    {
        public LinPermission(string auth, string module, int groupId)
        {
            Name = auth ?? throw new ArgumentNullException(nameof(auth));
            Module = module ?? throw new ArgumentNullException(nameof(module));
            GroupId = groupId;
        }

        public LinPermission()
        {
        }

        public LinPermission(string auth, int groupId)
        {
            GroupId = groupId;
            Name = auth ?? throw new ArgumentNullException(nameof(auth));
        }

        /// <summary>
        /// 权限字段
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// 所属权限、权限名称，例如：访问首页
        /// </summary>
        [Column(DbType = "varchar(60)")]
        public string Name { get; set; }

        /// <summary>
        /// 权限所属模块，例如：人员管理
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Module { get;  set; }

        public override string ToString()
        {
            return base.ToString() + $" Auth:{Name}、Module:{Module}";
        }
    }

}
