using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities
{
    [Table(Name = "lin_permission")]
    public class LinPermission : FullAduitEntity<long>
    {
        public LinPermission(string name, string module, string router)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Module = module ?? throw new ArgumentNullException(nameof(module));
            Router = router ?? throw new ArgumentNullException(nameof(router));
        }

        public LinPermission()
        {
        }

        /// <summary>
        /// 所属权限、权限名称，例如：访问首页
        /// </summary>
        [Column(StringLength = 60)]
        public string Name { get; set; }

        /// <summary>
        /// 权限所属模块，例如：人员管理
        /// </summary>
        [Column(StringLength = 50)]
        public string Module { get; set; }

        /// <summary>
        /// 后台路由
        /// </summary>
        [Column(StringLength = 200)]
        public string Router { get; set; }

        public override string ToString()
        {
            return base.ToString() + $" Auth:{Name}、Module:{Module}";
        }
    }

}
