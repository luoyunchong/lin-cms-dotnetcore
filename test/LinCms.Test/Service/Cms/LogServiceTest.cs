using System.Collections.Generic;
using LinCms.Cms.Logs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace LinCms.Test.Service.Cms
{

    public class LogServiceTest 
    {
        private readonly ILogService _logService;

        public LogServiceTest(ILogService logService) : base()
        {
            _logService = logService;
        }

        [Fact]
        public void GetLoggedUsersTest()
        {
            List<string> users = _logService.GetLoggedUsers(null);
        }
    }
}
