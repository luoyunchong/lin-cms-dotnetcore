using System.Threading.Tasks;
using AutoMapper;
using LinCms.Blog.Articles;
using LinCms.Controllers.Blog;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Controller.v1
{
    public class ArticleControllerTest : BaseControllerTests
    {
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly ArticleController _articleController;

        public ArticleControllerTest() : base()
        {
            _articleController = ServiceProvider.GetService<ArticleController>(); ;

            _mapper = ServiceProvider.GetService<IMapper>();
            _freeSql = ServiceProvider.GetService<IFreeSql>();
        }



        [Fact]
        public async Task CreateAsync()
        {
            CreateUpdateArticleDto createArticle = new CreateUpdateArticleDto();
            await _articleController.CreateAsync(createArticle);
        }


    }
}
