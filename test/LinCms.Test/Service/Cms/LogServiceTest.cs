using System.Collections.Generic;
using LinCms.Cms.Logs;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
namespace LinCms.Test.Service.Cms
{

    public class LogServiceTest : BaseLinCmsTest
    {
        private readonly ILogService _logService;

        public LogServiceTest() : base()
        {
            _logService = ServiceProvider.GetService<ILogService>();
        }

        [Fact]
        public void GetLoggedUsersTest()
        {
            List<string> users = _logService.GetLoggedUsers(null);
        }
    }
}
