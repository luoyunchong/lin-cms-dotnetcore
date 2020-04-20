using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Utils
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public class ReflexHelper
    {
        public static List<T> GetAssembly<T>() where T : Attribute
        {
            List<T> listT = new List<T>();
            List<Type> assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(d =>
            {
                T linCmsAuthorize = d.GetCustomAttribute<T>();
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
        /// 通过反射得到LinCmsAttrbutes所有权限结构，为树型权限生成做准备
        /// </summary>
        /// <returns></returns>
        public static List<PermissionDefinition> GeAssemblyLinCmsAttributes()
        {
            List<PermissionDefinition> linAuths = new List<PermissionDefinition>();

            List<Type> assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
            //通过反射得到控制器上的权限特性标签
            assembly.ForEach(d =>
            {
                LinCmsAuthorizeAttribute linCmsAuthorize = d.GetCustomAttribute<LinCmsAuthorizeAttribute>();
                RouteAttribute routerAttribute = d.GetCustomAttribute<RouteAttribute>();
                if (linCmsAuthorize?.Permission != null && routerAttribute?.Template != null)
                {
                    linAuths.Add(new PermissionDefinition(linCmsAuthorize.Permission, linCmsAuthorize.Module, routerAttribute.Template.Replace("/", ".")));
                }
            });

            //得到方法上的权限特性标签，并排除无权限及模块的非固定权限
            assembly.ForEach(r =>
            {
                RouteAttribute routerAttribute = r.GetCustomAttribute<RouteAttribute>();
                if (routerAttribute?.Template != null)
                {
                    foreach (MethodInfo methodInfo in r.GetMethods())
                    {
                        foreach (Attribute attribute in methodInfo.GetCustomAttributes())
                        {
                            if (attribute is LinCmsAuthorizeAttribute linCmsAuthorize && linCmsAuthorize.Permission.IsNotNullOrEmpty() && linCmsAuthorize.Module.IsNotNullOrEmpty())
                            {
                                linAuths.Add(
                                        new PermissionDefinition(
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

        /// <summary>
        /// 使用动态 ExpandoObject结构实现前台需要的奇怪的JSON格式
        /// </summary>
        /// <returns></returns>
        public static dynamic AuthorizationConvertToTree(List<PermissionDefinition> listPermission)
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

            IDictionary<string, object> expandoObject = new ExpandoObject();
            foreach (var permission in permissionTree)
            {
                IDictionary<string, object> perExpandObject = new ExpandoObject();

                foreach (var children in permission.Children)
                {
                    perExpandObject[children.Permission] = new List<string> { children.Router };
                }

                expandoObject[permission.Key] = perExpandObject;
            }

            return expandoObject;
        }

        public static List<IDictionary<string, object>> AuthsConvertToTree(List<LinPermission> listAuths)
        {
            var groupAuths = listAuths.GroupBy(r => r.Module).Select(r => new
            {
                r.Key,
                Children = r.Select(u => u.Name).ToList()
            }).ToList();

            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();

            foreach (var groupAuth in groupAuths)
            {
                IDictionary<string, object> moduleExpandoObject = new ExpandoObject();
                List<IDictionary<string, object>> perExpandList = new List<IDictionary<string, object>>();
                groupAuth.Children.ForEach(permission =>
                {
                    IDictionary<string, object> perExpandObject = new ExpandoObject();
                    perExpandObject["module"] = groupAuth.Key;
                    perExpandObject["permission"] = permission;
                    perExpandList.Add(perExpandObject);
                });

                moduleExpandoObject[groupAuth.Key] = perExpandList;
                list.Add(moduleExpandoObject);
            }

            return list;
        }

        /// <summary>
        /// 扫描 IEntity类所在程序集，反射得到所有类上有特性标签为TableAttribute
        /// </summary>
        /// <returns></returns>
        public static Type[] GetEntityTypes(Type type)
        {
            Type[] tableAssembies = Assembly.GetAssembly(type).GetExportedTypes().Where(o =>
            {
                foreach (Attribute a in Attribute.GetCustomAttributes(o, true))
                {
                    if (a is TableAttribute)
                        return true;
                }
                return false;

            }).ToArray();
            return tableAssembies;
        }
    }
}
