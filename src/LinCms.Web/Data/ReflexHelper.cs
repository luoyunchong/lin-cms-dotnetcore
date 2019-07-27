using System;
using System.Collections.Generic;
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
    public class ReflexHelper
    {
    
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


        //LinCms.Zero.Data.PermissionDto Permission:查询日志记录的用户、Module:日志、Router:cms.log+get_users
        //LinCms.Zero.Data.PermissionDto Permission:查询所有日志、Module:日志、Router:cms.log+get_logs
        //LinCms.Zero.Data.PermissionDto Permission:搜索日志、Module:日志、Router:cms.log+get_user_logs
        //LinCms.Zero.Data.PermissionDto Permission:查看lin的信息、Module:信息、Router:cms.test+info
        //LinCms.Zero.Data.PermissionDto Permission:删除图书、Module:图书、Router:v1.book+delete_book

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

        public static void AuthorizationConvertToTree(List<PermissionDto> listPermission)
        {
            var permissionTree = listPermission.GroupBy(r => r.Module).Select(r => new
            {
                r.Key,
                v = r.Select(u => new
                {
                    u.Module,
                    u.Permission
                })
            }).ToList();



        }
    }
}
