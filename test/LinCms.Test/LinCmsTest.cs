using System;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FreeSql;
using LinCms.Web;
using LinCms.Web.Configs;
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
        public LinCmsTest(ITestOutputHelper testOut)
        {
            testOutputHelper = testOut;
        }
        [Fact]
        public void OutputTest()
        {
            testOutputHelper.WriteLine("BaseLinCmsTest ConfigureServices");
        }
    }
}
