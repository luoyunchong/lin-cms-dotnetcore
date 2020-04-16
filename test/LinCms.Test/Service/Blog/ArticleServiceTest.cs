using System;
using System.Threading.Tasks;
using LinCms.Application.Blog.Articles;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Application.Contracts.Blog.Articles.Dtos;
using LinCms.Core.Data;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FreeSql;

namespace LinCms.Test.Service.Blog
{
    public class ArticleServiceTest : BaseLinCmsTest
    {
        private readonly IArticleService _articleService;
        private readonly IUnitOfWork _unitOfWork;

        public ArticleServiceTest() : base()
        {
            _articleService = GetService<IArticleService>();
            _unitOfWork = GetService<IUnitOfWork>();
            ;
        }

        [Fact]
        public async Task DeleteAsync()
        {
            await _articleService.DeleteAsync(new Guid("5dc93286-5e44-c190-008e-3fc74d4fcee0"));
            _unitOfWork.Commit();
        }

        [Fact]
        public async Task GetAsync()
        {
            string redisKey = "article:query:TestGetArticleAsync";
            var d = await RedisHelper.CacheShellAsync(redisKey, 3600,
                async () => { return await _articleService.GetAsync(new Guid("5deea988-b280-dff8-003c-85247943caf7")); }
            );
        }

        [Fact]
        public void GetSubscribeArticle()
        {
            string redisKey = "article:query:GetSubscribeArticle";
            var subscribute = RedisHelper.CacheShell(redisKey, 3600,
                async () => { return await _articleService.GetSubscribeArticleAsync(new PageDto()); }
            );
        }

        [Fact]
        public async Task GetArticleAsync()
        {
            string redisKey = "article:query:GetArticleAsync";
            var subscribute = await RedisHelper.CacheShellAsync(redisKey, 3600,
                async () => { return await _articleService.GetArticleAsync(new ArticleSearchDto()); }
            );
        }
    }
}