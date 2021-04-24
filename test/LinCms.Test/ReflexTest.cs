using FreeSql;
using LinCms.Aop.Filter;
using LinCms.Controllers.Cms;
using LinCms.Data;
using LinCms.FreeSql;
using LinCms.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
            MethodInfo[] controllerMethods = t.GetMethods();
            StringBuilder methodsNameAppend = new StringBuilder();

            foreach (MethodInfo t1 in controllerMethods)
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

            foreach (MethodInfo t1 in controllerMethods)
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

            foreach (Attribute attribute in attributes)
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
            List<Type> assembly = typeof(Startup.Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(r =>
            {
                foreach (MethodInfo methodInfo in r.GetMethods())
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
            List<Type> assembly = typeof(Startup.Startup).Assembly.GetTypes().AsEnumerable()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type)).ToList();

            assembly.ForEach(d =>
            {
                LinCmsAuthorizeAttribute linCmsAuthorize = d.GetCustomAttribute<LinCmsAuthorizeAttribute>();
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
            List<PermissionDefinition> attributes = ReflexHelper.GetAssemblyLinCmsAttributes();
            foreach (PermissionDefinition attribute in attributes)
            {
                _testOutputHelper.WriteLine(attribute.ToString());
            }
        }

        //LinCms.Zero.Domain.LinPermission Auth:查询日志记录的用户、Module:日志
        //LinCms.Zero.Domain.LinPermission Auth:查询所有日志、Module:日志
        //LinCms.Zero.Domain.LinPermission Auth:搜索日志、Module:日志
        //LinCms.Zero.Domain.LinPermission Auth:查看lin的信息、Module:信息
        [Fact]
        public void ConvertToTree()
        {
            List<PermissionDefinition> linCmsAttributes = ReflexHelper.GetAssemblyLinCmsAttributes();

            dynamic obj = ReflexHelper.AuthorizationConvertToTree(linCmsAttributes);

            string jsonSerializeObject = JsonConvert.SerializeObject(obj);

        }


        [Fact]
        public void Test()
        {
            List<Assembly> assem = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName.Contains("LinCms.")).ToList();
        }

        [Fact]
        public void RelfexGetCustomAttributes()
        {
            var entityTypes = ReflexHelper.GetTypesByTableAttribute();
            Assert.True(entityTypes.Length > 0);
        }

        [Fact]
        public void GetTypes()
        {
            var entityTypes = ReflexHelper.GetTypesByNameSpace();
            Assert.True(entityTypes.Length > 0);
        }

        [Fact]
        public void FreeSqlBuilderTest()
        {
            FreeSqlBuilder fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, "Data Source=localhost;Port=3306;User ID=root;Password=root;Initial Catalog=lincms2;Charset=utf8mb4;SslMode=none;Max pool size=1;Connection LifeTime=20")
                .UseAutoSyncStructure(true)
                .UseNoneCommandParameter(true);


            Type type = fsql.GetType();
            FieldInfo fieldInfo = type.GetField("_masterConnectionString", BindingFlags.NonPublic | BindingFlags.Instance);
            string value = (string)fieldInfo.GetValue(fsql);

            FieldInfo _dataTypeFieldInfo = type.GetField("_dataType", BindingFlags.NonPublic | BindingFlags.Instance);
            DataType dataType = (DataType)_dataTypeFieldInfo.GetValue(fsql);


        }


        [Fact]
        public void OdbcFreeSqlBuilderTest()
        {

            //using OdbcConnection cnn = new OdbcConnection("Driver={SQL Server};Server=.;Initial Catalog=master;Uid=sa;Pwd=123456");
            //cnn.Open();

            FreeSqlBuilder fsqlBuilder = new FreeSqlBuilder()
                .UseConnectionString(DataType.OdbcSqlServer, "Driver={SQL Server};AttachDBFilename=d:\\db\\File\\AdventureWorks.mdf;Server=.;Initial Catalog=AdventureWorks;Uid=sa;Pwd=123456")
                .UseAutoSyncStructure(true)
                .UseNoneCommandParameter(true);


            fsqlBuilder.CreateDatabaseIfNotExists_ODBCSqlServer();

        }

        [Fact]
        public void OdbcFreeSqlBuilderTest2()
        {

            FreeSqlBuilder fsqlBuilder2 = new FreeSqlBuilder()
               .UseConnectionString(DataType.OdbcSqlServer, "Driver={SQL Server};AttachDBFilename=|DataDirectory|\\AdventureWorks2.mdf;Server=.;Initial Catalog=AdventureWorks2;Uid=sa;Pwd=123456");

            fsqlBuilder2.CreateDatabaseIfNotExists_ODBCSqlServer();
        }


        /// <summary>
        ///  Message:  System.Data.Odbc.OdbcException : ERROR[IM002][Microsoft][ODBC Driver Manager] Data source name not found and no default driver specified
        /// </summary>
        [Fact]
        public void CreateDatabaseIfNotExists_ODBCMySql_Test()
        {
            FreeSqlBuilder fsqlBuilder2 = new FreeSqlBuilder()
               .UseConnectionString(DataType.OdbcMySql, "Driver={MySQL ODBC 8.0 Unicode Driver}; Server=127.0.0.1;Port=3308;Persist Security Info=False; Trusted_Connection=Yes;UID=root;PWD=123456; DATABASE=cccddd_odbc;Charset=utf8; SslMode=none;Min Pool Size=1");

            fsqlBuilder2.CreateDatabaseIfNotExists_ODBCMySql();

        }
    }
}
