using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LinCms.Zero.Authorization;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Data
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public class ReflexHelper
    {
        public static List<T> GetAssembly<T>() where T : Attribute
        {
            List<T> listT = new List<T>();
            var assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(d =>
            {
                var linCmsAuthorize = d.GetCustomAttribute<T>();
                listT.Add(linCmsAuthorize);
            });
            return listT;
        }


        //得到这样的结果
        //LinCms.Zero.Data.PermissionDto Permission:查询日志记录的用户、Module:日志、Router:cms.log+get_users
        //LinCms.Zero.Data.PermissionDto Permission:查询所有日志、Module:日志、Router:cms.log+get_logs
        //LinCms.Zero.Data.PermissionDto Permission:搜索日志、Module:日志、Router:cms.log+get_user_logs
        //LinCms.Zero.Data.PermissionDto Permission:查看lin的信息、Module:信息、Router:cms.test+info
        //LinCms.Zero.Data.PermissionDto Permission:删除图书、Module:图书、Router:v1.book+delete_book
        /// <summary>
        /// 为树型权限生成做准备
        /// </summary>
        /// <returns></returns>
        public static List<PermissionDto> GeAssemblyLinCmsAttributes()
        {
            List<PermissionDto> linAuths = new List<PermissionDto>();

            var assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(d =>
            {
                var linCmsAuthorize = d.GetCustomAttribute<LinCmsAuthorizeAttribute>();
                var routerAttribute = d.GetCustomAttribute<RouteAttribute>();
                if (linCmsAuthorize?.Permission != null && routerAttribute?.Template != null)
                {
                    linAuths.Add(new PermissionDto(linCmsAuthorize.Permission, linCmsAuthorize.Module, routerAttribute.Template.Replace("/", ".")));
                }
            });

            assembly.ForEach(r =>
            {
                var routerAttribute = r.GetCustomAttribute<RouteAttribute>();
                if (routerAttribute?.Template != null)
                {
                    foreach (var methodInfo in r.GetMethods())
                    {
                        foreach (Attribute attribute in methodInfo.GetCustomAttributes())
                        {
                            if (attribute is LinCmsAuthorizeAttribute linCmsAuthorize)
                            {
                                linAuths.Add(
                                        new PermissionDto(
                                                linCmsAuthorize.Permission,
                                                linCmsAuthorize.Module,
                                                $"{routerAttribute.Template.Replace("/", ".")}+{methodInfo.Name.ToSnakeCase()}"
                                                )
                                    );
                            }
                        }
                    }
                }
            });

            return linAuths;
        }

        /**
             {
                    "信息":{
                        "查看lin的信息":[
                            "cms.test+info"
                        ]
                    },
                    "图书":{
                        "删除图书":[
                            "v1.book+delete_book"
                        ]
                    },
                    "日志":{
                        "搜索日志":[
                            "cms.log+get_user_logs"
                        ],
                        "查询所有日志":[
                            "cms.log+get_logs"
                        ],
                        "查询日志记录的用户":[
                            "cms.log+get_users"
                        ]
                    }
            }
             */

        public static dynamic AuthorizationConvertToTree(List<PermissionDto> listPermission)
        {
            var permissionTree = listPermission.GroupBy(r => r.Module).Select(r => new
            {
                r.Key,
                Children = r.Select(u => new
                {
                    u.Router,
                    u.Permission
                }).ToList()
            }).ToList();

            IDictionary<string, object> expandoObject = new ExpandoObject() as IDictionary<string, object>;
            foreach (var permission in permissionTree)
            {
                IDictionary<string, object> perExpandObject = new ExpandoObject() as IDictionary<string, object>;

                foreach (var children in permission.Children)
                {
                       perExpandObject[children.Permission] = new List<string> {children.Router};
                }

                expandoObject[permission.Key] = perExpandObject;
            }

            return expandoObject;
        }
    }
}
