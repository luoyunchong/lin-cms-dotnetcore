using LinCms.Core.Security;

namespace LinCms.Application
{

    public abstract class AppService: IAppService
    {
        protected CurrentUser CurrentUser;

        protected AppService(CurrentUser currentUser)
        {
            CurrentUser = currentUser;
        }

    }
}
