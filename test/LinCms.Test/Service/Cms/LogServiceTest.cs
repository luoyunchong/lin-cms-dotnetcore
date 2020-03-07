using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Blog.Articles;
using LinCms.Application.Cms.Logs;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Core.Data;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
namespace LinCms.Test.Service.Cms
{

    public class LogServiceTest : BaseLinCmsTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogService _logService;

        public LogServiceTest(ITestOutputHelper testOutputHelper) : base()
        {
            _logService = ServiceProvider.GetService<ILogService>();
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetLoggedUsersTest()
        {
            List<string> users = _logService.GetLoggedUsers(null);
        }
    }
}
