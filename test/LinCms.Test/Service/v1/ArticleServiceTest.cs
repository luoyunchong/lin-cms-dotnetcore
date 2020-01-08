using System;
using System.Collections.Generic;
using System.Text;
using LinCms.Application.v1.Articles;
using LinCms.Test.Repositories;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Service.v1
{
    public class ArticleServiceTest : BaseRepositoryTest
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
    }
}
