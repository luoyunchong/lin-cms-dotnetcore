using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Blog.UserSubscribes;

namespace LinCms.Blog.Tags;

/// <summary>
/// 标签
/// </summary>
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

    /// <summary>
    /// 修改标签下文章数量
    /// </summary>
    /// <param name="id"></param>
    /// <param name="inCreaseCount"></param>
    /// <returns></returns>
    Task UpdateArticleCountAsync(Guid? id, int inCreaseCount);

    /// <summary>
    /// 更新标签下订阅数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="inCreaseCount"></param>
    /// <returns></returns>
    Task UpdateSubscribersCountAsync(Guid? id, int inCreaseCount);

    /// <summary>
    /// 标签-校正标签对应随笔数量
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    Task CorrectedTagCountAsync(Guid tagId);

    /// <summary>
    /// 标签浏览量+1
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    Task IncreaseTagViewHits(Guid tagId);
}