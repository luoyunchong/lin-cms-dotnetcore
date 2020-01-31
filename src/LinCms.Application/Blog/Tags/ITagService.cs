using System;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Core.Data;

namespace LinCms.Application.Blog.Tags
{
    public interface ITagService
    {
        bool IsSubscribe(Guid tagId);
         PagedResultDto<TagDto> Get(TagSearchDto searchDto);
         PagedResultDto<TagDto> GetSubscribeTags(UserSubscribeSearchDto userSubscribeDto);
         void UpdateArticleCount(Guid? id, int inCreaseCount);

         void UpdateSubscribersCount(Guid? id, int inCreaseCount);
    }
}
