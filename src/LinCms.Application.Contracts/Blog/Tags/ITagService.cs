using System;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Tags.Dtos;
using LinCms.Application.Contracts.Blog.UserSubscribes.Dtos;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Blog.Tags
{
    public interface ITagService
    {
        Task CreateAsync(CreateUpdateTagDto createTag);

        Task UpdateAsync(Guid id, CreateUpdateTagDto updateTag);

        Task<TagListDto> GetAsync(Guid id);

        Task<PagedResultDto<TagListDto>> GetListAsync(TagSearchDto searchDto);

        /// <summary>
        /// 判断标签是否被关注
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        Task<bool> IsSubscribeAsync(Guid tagId);

        /// <summary>
        /// 得到某个用户关注的标签
        /// </summary>
        /// <param name="userSubscribeDto"></param>
        /// <returns></returns>
        PagedResultDto<TagListDto> GetSubscribeTags(UserSubscribeSearchDto userSubscribeDto);

        void UpdateArticleCount(Guid? id, int inCreaseCount);

        void UpdateSubscribersCount(Guid? id, int inCreaseCount);

        Task CorrectedTagCountAsync(Guid tagId);

        /// <summary>
        /// 标签浏览量+1
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        Task IncreaseTagViewHits(Guid tagId);
    }
}
