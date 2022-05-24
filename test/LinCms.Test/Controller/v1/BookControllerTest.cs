using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.v1.Books;
using Newtonsoft.Json;
using Xunit;

namespace LinCms.Test.Controller.v1
{
    public class BookControllerTest : BaseControllerTests
    {
        public BookControllerTest() : base()
        {

        }
        [Fact]

        public async Task HttpClientResourePasswordTest()
        {
            await HttpClientResourePassword();
        }

        [Fact]
        public async Task CreateAsync()
        {
            await HttpClientResourePassword();
            // Act
            CreateUpdateBookDto createUpdateBookDto = new CreateUpdateBookDto()
            {
                Author = "luo",
                Summary = "lin-cms-dotnetcore从零到1是一本介绍一个SB开发的一个小系统",
                Title = "lin-cms-dotnetcore从零到1",
                Image = "123"
            };

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(createUpdateBookDto));

            var response = await Client.PostAsync($"/v1/book", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PutAsync()
        {
            // Act
            CreateUpdateBookDto createUpdateBookDto = new CreateUpdateBookDto()
            {
                Author = "luo",
                Summary = "lin-cms-dotnetcore从零到1是一本介绍一个SB开发的一个小系统",
                Title = "lin-cms-dotnetcore从零到1"
            };

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(createUpdateBookDto));

            var response = await Client.PostAsync($"/v1/book", httpContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetListAsync()
        {
            // 1、Arrange
            PageDto pageDto = new PageDto();

            // Act
            var response = await Client.GetAsync($"/v1/book?{ToParams(pageDto)}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }



    }
}
