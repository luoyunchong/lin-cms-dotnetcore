using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Test
{
    public class TestStartup 
    {
        public IConfiguration Configuration { get; }

        public TestStartup(IConfiguration configuration)
        {
        }

        public void ConfigureTestServices(IServiceCollection services)
        {

        }
    }
}
