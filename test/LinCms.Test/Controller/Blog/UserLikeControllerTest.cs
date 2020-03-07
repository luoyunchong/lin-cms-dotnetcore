using System;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Application.Contracts.Blog.UserLikes;
using LinCms.Core.Entities.Blog;
using LinCms.Web.Controllers.Blog;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace LinCms.Test.Controller.Blog
{
    public class UserLikeControllerTest : BaseControllerTests
    {
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly UserLikeController _userLikeController;
        private readonly ITestOutputHelper _testOutputHelper;

        public UserLikeControllerTest(ITestOutputHelper testOutputHelper) : base()
        {
            _userLikeController = serviceProvider.GetService<UserLikeController>(); ;

            _mapper = serviceProvider.GetService<IMapper>();
            _freeSql = serviceProvider.GetService<IFreeSql>();
            _testOutputHelper = testOutputHelper;
        }



        [Fact]
        public void Create()
        {
            CreateUpdateUserLikeDto createUpdateUserLike = new CreateUpdateUserLikeDto()
            {
                SubjectId = new Guid("5e63dcd7-6e39-36e0-0001-059272461091"),
                SubjectType = UserLikeSubjectType.UserLikeComment
            };
            _userLikeController.Create(createUpdateUserLike);
        }


    }
}
