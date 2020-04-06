using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Test.Controller.v1
{
    public class BaseTypeControllerTest:BaseControllerTests
    {
        private readonly IWebHostEnvironment _hostingEnv;

        public BaseTypeControllerTest() : base()
        {
            _hostingEnv = ServiceProvider.GetService<IWebHostEnvironment>();
        }



        public void Gets()
        {

        }

    }
}
