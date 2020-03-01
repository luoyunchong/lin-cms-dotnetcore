using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Blog.Articles;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Core.Data;
using LinCms.Infrastructure.Repositories;
using LinCms.Test.Repositories;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Service.v1
{
    public class ArticleServiceTest : BaseLinCmsTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IArticleService _articleService;

        public ArticleServiceTest(ITestOutputHelper testOutputHelper) : base()
        {
            _articleService = ServiceProvider.GetService<IArticleService>();
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Delete()
        {
            _articleService.Delete(new Guid("5dc93286-5e44-c190-008e-3fc74d4fcee0"));
        }

        [Fact]
        public async Task GetAsync()
        {
            string redisKey = "article:query:TestGetArticleAsync";
            var d =await RedisHelper.CacheShellAsync(redisKey, 3600, async () =>
                    {
                        return await _articleService.GetAsync(new Guid("5deea988-b280-dff8-003c-85247943caf7"));
                    }
               );

        }

        [Fact]
        public void GetSubscribeArticle()
        {
            string redisKey = "article:query:GetSubscribeArticle";
            var subscribute = RedisHelper.CacheShell(redisKey, 3600, () =>
                {
                    return _articleService.GetSubscribeArticle(new PageDto());
                }
            );

        }

        [Fact]
        public async Task GetArticleAsync()
        {
            string redisKey = "article:query:GetArticleAsync";
            var subscribute =await RedisHelper.CacheShellAsync(redisKey, 3600, async () =>
                {
                    return await _articleService.GetArticleAsync(new ArticleSearchDto());
                }
            );

        }
    }
}
