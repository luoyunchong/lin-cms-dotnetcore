using AutoMapper;
using LinCms.Web.Controllers.v1;
using LinCms.Web.Models.v1.Articles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace LinCms.Test.Controller.v1
{
    public class ArticleControllerTest : BaseControllerTests
    {
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly ArticleController _articleController;
        private readonly ITestOutputHelper _testOutputHelper;

        public ArticleControllerTest(ITestOutputHelper testOutputHelper) : base()
        {
            _articleController = serviceProvider.GetService<ArticleController>(); ;

            _mapper = serviceProvider.GetService<IMapper>();
            _freeSql = serviceProvider.GetService<IFreeSql>();
            _testOutputHelper =testOutputHelper;
        }



        [Fact]
        public void Post()
        {
            CreateUpdateArticleDto createArticle = new CreateUpdateArticleDto();
            _articleController.Post(createArticle);
        }


    }
}
