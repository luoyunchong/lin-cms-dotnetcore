using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FreeSql;
using LinCms.Web;
using LinCms.Web.Configs;
using LinCms.Web.Uow;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
namespace LinCms.Test
{
    public class LinCmsTest : BaseLinCmsTest
    {
        ITestOutputHelper testOutputHelper;
        private readonly IFreeSql freeSql;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        public LinCmsTest(ITestOutputHelper testOut)
        {
            testOutputHelper = testOut;
            freeSql = GetService<IFreeSql>();
            unitOfWorkManager = GetService<IUnitOfWorkManager>();
        }
        [Fact]
        public void OutputTest()
        {
            testOutputHelper.WriteLine("BaseLinCmsTest ConfigureServices");
        }

        /// <summary>
        /// 主要负责工作单元的创建
        /// </summary>
        [Fact]
        public void CreateUnitOfWorkTest()
        {
            using (IUnitOfWork uow = freeSql.CreateUnitOfWork())
            {
                uow.GetOrBeginTransaction();

                using (IUnitOfWork uow2 = freeSql.CreateUnitOfWork())
                {
                    uow2.GetOrBeginTransaction();

                    uow2.Commit();
                }
                uow.Commit();
            }

        }

        [Fact]
        public void CreateUnitOfWorkManagerTest()
        {
            using (IUnitOfWork uow = unitOfWorkManager.Begin())
            {
                using (IUnitOfWork uow2 = unitOfWorkManager.Begin())
                {
                    uow2.Commit();
                }
                uow.Commit();
            }

        }
    }
}
