using AutoMapper;
using LinCms.Web.Models.v1.Comments;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.AutoMapper.v1
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<CreateCommentDto, Comment>();
            CreateMap<Comment, CommentDto>();
        }
    }
}
