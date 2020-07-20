using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Entities
{
    [Table(Name = "lin_group")]
    public class LinGroup : Entity<long>
    {
        public LinGroup()
        {
        }

        public LinGroup(string name, string info, bool isStatic)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Info = info ?? throw new ArgumentNullException(nameof(info));
            IsStatic = isStatic;
        }

        /// <summary>
        /// 权限组名称
        /// </summary>
        [Column(StringLength = 60)]
        public string Name { get; set; }
        /// <summary>
        /// 权限组描述
        /// </summary>
        [Column(StringLength = 100)]
        public string Info { get; set; }

        /// <summary>
        /// 是否是静态分组
        /// </summary>
        public bool IsStatic { get; set; } = false;

        [Navigate(ManyToMany = typeof(LinUserGroup))]
        public virtual ICollection<LinUser> LinUsers { get; set; }

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
