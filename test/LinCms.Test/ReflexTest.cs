using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinCms.Web;
using LinCms.Web.Controllers;
using LinCms.Web.Data;
using LinCms.Zero.Authorization;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Xunit.Abstractions;

namespace LinCms.Test
{
    public class ReflexTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ReflexTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetAllActionByController()
        {
            Type t = typeof(LogController);
            System.Reflection.MethodInfo[] controllerMethods = t.GetMethods();
            StringBuilder methodsNameAppend = new StringBuilder();

            foreach (var t1 in controllerMethods)
            {
                methodsNameAppend.Append(t1.Name + ";");
                _testOutputHelper.WriteLine(t1.Name);
            }
        }

        [Fact]
        public void TestOutputHelpler()
        {
            _testOutputHelper.WriteLine("console test output helper");
        }


        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","查询日志记录的用户","日志"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","查询所有日志","日志"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","搜索日志","日志"
        [Fact]
        public void LogControllerGetLinCmsAuthorize()
        {
            Type t = typeof(LogController);
            MethodInfo[] controllerMethods = t.GetMethods();

            foreach (var t1 in controllerMethods)
            {
                IEnumerable<Attribute> v = t1.GetCustomAttributes();

                foreach (Attribute v1 in v)
                {
                    if (v1 is LinCmsAuthorizeAttribute vv)
                    {
                        _testOutputHelper.WriteLine(vv.ToString());
                    }
                }
            }
        }

        //Microsoft.AspNetCore.Mvc.RouteAttribute
        //Microsoft.AspNetCore.Mvc.ApiControllerAttribute
        //Microsoft.AspNetCore.Mvc.ControllerAttribute
        [Fact]
        public void GetLinCmsAttribute()
        {
            IEnumerable<Attribute> attributes = typeof(LogController).GetTypeInfo().GetCustomAttributes();

            foreach (var attribute in attributes)
            {
                _testOutputHelper.WriteLine(attribute.ToString());
            }
        }


        /// <summary>
        /// 获取方法上的LinCmsAuthorizeAttribute
        /// </summary>
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:查询日志记录的用户","Module:日志","Roles:","Policy:","AuthenticationSchemes:"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:查询所有日志","Module:日志","Roles:","Policy:","AuthenticationSchemes:"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:搜索日志","Module:日志","Roles:","Policy:","AuthenticationSchemes:"
        //"LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:查看lin的信息","Module:信息","Roles:","Policy:","AuthenticationSchemes:"
        [Fact]
        public void GetAssemblyMethodsAttributes()
        {
            var assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(r =>
            {
                foreach (var methodInfo in r.GetMethods())
                {
                    foreach (Attribute attribute in methodInfo.GetCustomAttributes())
                    {
                        if (attribute is LinCmsAuthorizeAttribute linCmsAuthorize)
                        {
                            _testOutputHelper.WriteLine(linCmsAuthorize.ToString());
                        }
                    }
                }
            });
        }


        /// <summary>
        /// 获取控制器上的LinCmsAuthorizeAttribute
        /// </summary>
        /// "LinCms.Zero.Authorization.LinCmsAuthorizeAttribute","Permission:","Module:","Roles:Administrator","Policy:","AuthenticationSchemes:"
        [Fact]
        public void GetControllerAttributes()
        {
            var assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(d =>
            {
                var linCmsAuthorize = d.GetCustomAttribute<LinCmsAuthorizeAttribute>();
                if (linCmsAuthorize != null)
                {
                    _testOutputHelper.WriteLine(linCmsAuthorize.ToString());
                }
            });
        }

        //LinCms.Zero.Data.PermissionDto Permission:查询日志记录的用户、Module:日志、Router:cms.log+get_users
        //LinCms.Zero.Data.PermissionDto Permission:查询所有日志、Module:日志、Router:cms.log+get_logs
        //LinCms.Zero.Data.PermissionDto Permission:搜索日志、Module:日志、Router:cms.log+get_user_logs
        //LinCms.Zero.Data.PermissionDto Permission:查看lin的信息、Module:信息、Router:.test+info
        [Fact]
        public void ReflexHelperTest()
        {
            List<PermissionDto> attributes = ReflexHelper.GeAssemblyLinCmsAttributes();
            foreach (var attribute in attributes)
            {
                _testOutputHelper.WriteLine(attribute.ToString());
            }
        }

        //LinCms.Zero.Domain.LinAuth Auth:查询日志记录的用户、Module:日志
        //LinCms.Zero.Domain.LinAuth Auth:查询所有日志、Module:日志
        //LinCms.Zero.Domain.LinAuth Auth:搜索日志、Module:日志
        //LinCms.Zero.Domain.LinAuth Auth:查看lin的信息、Module:信息
        [Fact]
        public  void ConvertToTree()
        {
            List<PermissionDto> listAuths = ReflexHelper.GeAssemblyLinCmsAttributes();
            var rr=listAuths.GroupBy(r => r.Module).Select(r => new
            {
                r.Key,
                Auths = r.Select(u => u.Permission).ToList()
            });

        }
    }
}
