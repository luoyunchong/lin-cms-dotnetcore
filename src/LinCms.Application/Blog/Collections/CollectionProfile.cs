using AutoMapper;
using LinCms.Blog.Collections;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Comments;

public class CollectionProfile:Profile
{
    public CollectionProfile()
    {
        CreateMap<CreateUpdateCollectionDto, Collection>();
        CreateMap<Collection, CommentDto>();

        CreateMap<CreateCancelArticleCollectionDto, ArticleCollection>();
    }
}