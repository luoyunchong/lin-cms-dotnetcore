using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Blog.Tags;
using LinCms.Blog.UserSubscribes;
using LinCms.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 用户关注标签
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/user-tag")]
[ApiController]
[Authorize]
public class UserTagController(ITagService tagService, IUserTagService userTagService) : ControllerBase
{
    /// <summary>
    /// 用户关注标签
    /// </summary>
    /// <param name="tagId"></param>
    [HttpPost("{tagId}")]
    public async Task<UnifyResponseDto> CreateUserTagAsync(Guid tagId)
    {
        await userTagService.CreateUserTagAsync(tagId);
        return UnifyResponseDto.Success("关注成功");
    }

    /// <summary>
    /// 取消关注标签
    /// </summary>
    /// <param name="tagId"></param>
    [HttpDelete("{tagId}")]
    public async Task<UnifyResponseDto> DeleteUserTagAsync(Guid tagId)
    {
        await userTagService.DeleteUserTagAsync(tagId);
        return UnifyResponseDto.Success("取消关注成功");
    }

    /// <summary>
    /// 用户关注的标签的分页数据
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public PagedResultDto<TagListDto> GetUserTagList([FromQuery] UserSubscribeSearchDto userSubscribeDto)
    {
        return tagService.GetSubscribeTags(userSubscribeDto);
    }
}