using LinCms.Application.Contracts.Blog.Articles.Dtos;
using LinCms.Application.Contracts.Cms.Books;
using LinCms.Application.Contracts.v1.Books.Dtos;
using LinCms.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
