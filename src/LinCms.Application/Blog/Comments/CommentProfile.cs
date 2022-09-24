using AutoMapper;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Comments;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CreateCommentDto, Comment>();
        CreateMap<Comment, CommentDto>().ForMember(d => d.TopComment, opts => opts.Ignore());
    }
}