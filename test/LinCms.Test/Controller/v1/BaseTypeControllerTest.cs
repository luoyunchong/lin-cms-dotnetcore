using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Test.Controller.v1
{
    public class BaseTypeControllerTest:BaseControllerTests
    {
        private readonly IHostingEnvironment _hostingEnv;

        public BaseTypeControllerTest() : base()
        {
            _hostingEnv = serviceProvider.GetService<IHostingEnvironment>();
        }



        public void Gets()
        {

        }

    }
}
