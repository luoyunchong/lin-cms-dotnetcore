using System;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Application.Contracts.Blog.UserLikes;
using LinCms.Application.Contracts.Blog.UserLikes.Dtos;
using LinCms.Core.Entities.Blog;
using LinCms.Web.Controllers.Blog;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace LinCms.Test.Controller.Blog
{
    public class UserLikeControllerTest : BaseControllerTests
    {

        public UserLikeControllerTest() : base()
        {
        }



        [Fact]
        public void Create()
        {
            CreateUpdateUserLikeDto createUpdateUserLike = new CreateUpdateUserLikeDto()
            {
                SubjectId = new Guid("5e63dcd7-6e39-36e0-0001-059272461091"),
                SubjectType = UserLikeSubjectType.UserLikeComment
            };
        }


    }
}
