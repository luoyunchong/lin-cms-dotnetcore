using System;
using LinCms.Blog.UserLikes;
using LinCms.Entities.Blog;
using Xunit;

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
