using System;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Blog.Articles;
using LinCms.Data;
using Xunit;

namespace LinCms.Test.Service.Blog
{
    public class ArticleServiceTest : BaseLinCmsTest
    {
        private readonly IArticleService _articleService;
        private readonly UnitOfWorkManager _unitOfWorkManager;

        public ArticleServiceTest() : base()
        {
            _articleService = GetService<IArticleService>();
            _unitOfWorkManager = GetService<UnitOfWorkManager>();
        }

        [Fact]
        public async Task DeleteAsync()
        {
            await _articleService.DeleteAsync(new Guid("5dc93286-5e44-c190-008e-3fc74d4fcee0"));
        }

        [Fact]
        public async Task GetAsync()
        {
            await _articleService.GetAsync(new Guid("5deea988-b280-dff8-003c-85247943caf7"));

        }

        [Fact]
        public async Task GetSubscribeArticleAsyncTest()
        {
            await _articleService.GetSubscribeArticleAsync(new PageDto());
        }

        [Fact]
        public async Task GetArticleAsync()
        {
            await _articleService.GetArticleAsync(new ArticleSearchDto() { TagId = Guid.NewGuid() });
        }
    }
}