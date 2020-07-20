using System.Threading.Tasks;
using LinCms.Data;
using LinCms.v1.Books;
using Xunit;

namespace LinCms.Test.Service.v1
{
    public class BookServiceTest : BaseLinCmsTest
    {
        private readonly IBookService _bookService;

        public BookServiceTest() : base()
        {
            _bookService = GetRequiredService<IBookService>();
        }

        [Fact]
        public async Task GetListAsyncTest()
        {
            PagedResultDto<BookDto> books = await _bookService.GetListAsync(new PageDto { });

            Assert.True(books.Items.Count > 0);
        }
    }
}
