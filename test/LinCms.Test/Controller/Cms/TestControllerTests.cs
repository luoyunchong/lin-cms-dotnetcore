using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace LinCms.Test.Controller.Cms
{
    public class TestControllerTests : BaseControllerTests
    {

        public TestControllerTests() : base()
        {

        }

        [Fact]
        public async Task Test_Info()
        {
            await base.HttpClientResourePassword();

            // Act
            var response = await Client.GetAsync($"/cms/test/info");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Test_Get()
        {
            // Act
            var response = await Client.GetAsync($"/cms/test/get");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
