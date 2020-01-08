using LinCms.Core.Security;

namespace LinCms.Application
{

    public abstract class AppService: IAppService
    {
        public CurrentUser CurrentUser;
    }
}
