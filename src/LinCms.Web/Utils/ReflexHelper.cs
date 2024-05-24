using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinCms.Aop.Filter;
using LinCms.Data;
using LinCms.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace LinCms.Utils;

/// <summary>
/// 反射帮助类
/// </summary>
public class ReflexHelper
{
    public static List<T> GetAssemblies<T>() where T : Attribute
    {
        List<T> listT = new List<T>();
        List<Type> assembly = typeof(Program).Assembly.GetTypes().AsEnumerable()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

        assembly.ForEach(d =>
        {
            T linCmsAuthorize = d.GetCustomAttribute<T>();
            listT.Add(linCmsAuthorize);
        });
        return listT;
    }


    //得到这样的结果
    //LinCms.Zero.Data.PermissionDto Permission:查询日志记录的用户、Module:日志、Router:cms.log.get_users
    //LinCms.Zero.Data.PermissionDto Permission:查询所有日志、Module:日志、Router:cms.log.get_logs
    //LinCms.Zero.Data.PermissionDto Permission:搜索日志、Module:日志、Router:cms.log.get_user_logs
    //LinCms.Zero.Data.PermissionDto Permission:查看lin的信息、Module:信息、Router:cms.test.info
    //LinCms.Zero.Data.PermissionDto Permission:删除图书、Module:图书、Router:v1.book.delete_book
    /// <summary>
    /// 通过反射得到LinCmsAttrbutes所有权限结构，为树型权限生成做准备
    /// </summary>
    /// <returns></returns>
    public static List<PermissionDefinition> GetAssemblyLinCmsAttributes()
    {
        List<PermissionDefinition> linAuths = new List<PermissionDefinition>();

        List<Type> assembly = typeof(Program).Assembly.GetTypes().AsEnumerable()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();
        //通过反射得到控制器上的权限特性标签
        assembly.ForEach(d =>
        {
            LinCmsAuthorizeAttribute linCmsAuthorize = d.GetCustomAttribute<LinCmsAuthorizeAttribute>();
            RouteAttribute routerAttribute = d.GetCustomAttribute<RouteAttribute>();
            if (linCmsAuthorize?.Permission != null && routerAttribute?.Template != null)
            {
                linAuths.Add(new PermissionDefinition(linCmsAuthorize.Permission, linCmsAuthorize.Module, routerAttribute.Template));
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
                    HttpMethodAttribute methodAttribute = GetMethodAttribute(methodInfo);

                    foreach (Attribute attribute in methodInfo.GetCustomAttributes())
                    {
                        if (attribute is LinCmsAuthorizeAttribute linAttribute &&
                            linAttribute.Permission.IsNotNullOrEmpty() && linAttribute.Module.IsNotNullOrEmpty())
                        {
                            string actionTemplate = methodAttribute.Template != null
                                ? "/" + methodAttribute.Template + " "
                                : " ";
                            string router =
                                $"{routerAttribute.Template}{actionTemplate}{methodAttribute.HttpMethods.FirstOrDefault()}";

                            // 判断 linAuths中的权限中是否已存在,如果存在，router增加现有router值

                            if (linAuths.Any(definition => definition.Permission == linAttribute.Permission))
                            {
                                PermissionDefinition permissionDefinition = linAuths.First(definition => definition.Permission == linAttribute.Permission);
                                permissionDefinition.Router += "," + router;
                            }
                            else
                            {

                                linAuths.Add(
                                    new PermissionDefinition(
                                        linAttribute.Permission,
                                        linAttribute.Module,
                                        router
                                    )
                                );
                            }
                        }
                    }
                }
            }
        });

        return linAuths.Distinct().ToList();
    }

    private static HttpMethodAttribute GetMethodAttribute(MethodInfo methodInfo)
    {
        HttpMethodAttribute methodAttribute = methodInfo.GetCustomAttribute<HttpGetAttribute>();
        if (methodAttribute != null)
        {
            return methodAttribute;
        }
        methodAttribute = methodInfo.GetCustomAttribute<HttpDeleteAttribute>();
        if (methodAttribute != null)
        {
            return methodAttribute;
        }
        methodAttribute = methodInfo.GetCustomAttribute<HttpPutAttribute>();
        if (methodAttribute != null)
        {
            return methodAttribute;
        }
        methodAttribute = methodInfo.GetCustomAttribute<HttpPostAttribute>();

        return methodAttribute;
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

        IDictionary<string, object> expandoObject = new Dictionary<string, object>();
        foreach (var permission in permissionTree)
        {
            IDictionary<string, object> perExpandObject = new Dictionary<string, object>();

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
        var groupAuths = listAuths.Where(r => r.PermissionType == PermissionType.Folder).GroupBy(r => r.Name).Select(r => new
        {
            r.Key,
            Children = r.Select(u => u.Name).ToList()
        }).ToList();

        List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();

        foreach (var groupAuth in groupAuths)
        {
            IDictionary<string, object> moduleExpandoObject = new Dictionary<string, object>();
            List<IDictionary<string, object>> perExpandList = new List<IDictionary<string, object>>();
            groupAuth.Children.ForEach(permission =>
            {
                IDictionary<string, object> perExpandObject = new Dictionary<string, object>();
                perExpandObject["module"] = groupAuth.Key;
                perExpandObject["permission"] = permission;
                perExpandList.Add(perExpandObject);
            });

            moduleExpandoObject[groupAuth.Key] = perExpandList;
            list.Add(moduleExpandoObject);
        }

        return list;
    }
}