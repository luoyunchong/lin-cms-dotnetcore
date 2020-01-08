using System;
using LinCms.Application.Contracts.v1.Tags;
using LinCms.Application.Contracts.v1.UserSubscribes;
using LinCms.Core.Data;

namespace LinCms.Application.v1.Tags
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
