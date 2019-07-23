using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain
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

        /// <summary>
        /// 信息
        /// </summary>
        public static class Test
        {
            /// <summary>
            /// 查看lin的信息
            /// </summary>
            public const string Info = "cms.test+info";
        }

        public static class Book
        {
            /// <summary>
            /// 删除图书
            /// </summary>
            public const string Delete = "v1.book+delete_book";
        }

        public static class Log
        {
            /// <summary>
            /// 搜索日志
            /// </summary>
            public const string GetUserLogs = "cms.log+get_user_logs";
            /// <summary>
            /// 查询所有日志
            /// </summary>
            public const string GetLogs = "cms.log+get_logs";
            /// <summary>
            /// 查询日志记录的用户
            /// </summary>
            public const string GetUsers = "cms.log+get_users";
        }

        public static List<ParentPermission> g()
        {
            var permissions = new List<ParentPermission>
            {
                new ParentPermission()
                {
                    ParentName = "信息",
                    Permissions=new List<Permission>
                    {
                        new Permission("查看Lin的信息",Test.Info)
                    }
                },
                 new ParentPermission()
                {
                    ParentName = "图书",
                    Permissions=new List<Permission>
                    {
                        new Permission("删除图书",Book.Delete)
                    }
                },
                  new ParentPermission()
                {
                    ParentName = "日志",
                    Permissions=new List<Permission>
                    {
                        new Permission("搜索日志",Log.GetUserLogs),
                        new Permission("查询所有日志",Log.GetLogs),
                        new Permission("查询日志记录的用户",Log.GetUsers),
                    }
                }
            };

            return permissions;


        }
    }



    public class Permission
    {
        public Permission(string name, string code)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public ParentPermission ParentPermission { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; }
    }

    public class ParentPermission
    {
        public string ParentName { get; set; }

        public List<Permission> Permissions { get; set; }
    }

}
