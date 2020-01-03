using System;
using LinCms.Web.Models.v1.Tags;
using LinCms.Web.Models.v1.UserSubscribes;
using LinCms.Zero.Data;

namespace LinCms.Web.Services.v1.Interfaces
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
