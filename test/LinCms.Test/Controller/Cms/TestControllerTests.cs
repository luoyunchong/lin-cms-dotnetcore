using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace LinCms.Test.Controller.Cms
{
    public class TestControllerTests : BaseControllerTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestControllerTests(ITestOutputHelper outputHelper) : base()
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task Test_Info()
        {
            await base.HttpClientResourePassword();

            // Act
            HttpResponseMessage response = await Client.GetAsync($"/cms/test/info");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Test_Get()
        {
            // Act
            HttpResponseMessage response = await Client.GetAsync($"/cms/test/get");

            // Output
            var responseTest = await response.Content.ReadAsStringAsync();
            _outputHelper.WriteLine(responseTest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
