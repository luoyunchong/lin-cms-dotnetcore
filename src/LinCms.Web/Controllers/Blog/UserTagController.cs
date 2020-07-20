using System;
using System.Threading.Tasks;
using LinCms.Blog.Tags;
using LinCms.Blog.UserSubscribes;
using LinCms.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog
{
    [Area("blog")]
    [Route("api/blog/user-tag")]
    [ApiController]
    [Authorize]
    public class UserTagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IUserTagService _userTagService;

        public UserTagController(ITagService tagService, IUserTagService userTagService)
        {
            _userTagService = userTagService;
            _tagService = tagService;
        }

        /// <summary>
        /// 用户关注标签
        /// </summary>
        /// <param name="tagId"></param>
        [HttpPost("{tagId}")]
        public async Task CreateUserTagAsync(Guid tagId)
        {
            await _userTagService.CreateUserTagAsync(tagId);
        }

        /// <summary>
        /// 取消关注标签
        /// </summary>
        /// <param name="tagId"></param>
        [HttpDelete("{tagId}")]
        public async Task DeleteUserTagAsync(Guid tagId)
        {
            await _userTagService.DeleteUserTagAsync(tagId);
        }

        /// <summary>
        /// 得到某个用户关注的标签
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public PagedResultDto<TagListDto> GetUserTagList([FromQuery] UserSubscribeSearchDto userSubscribeDto)
        {
            return _tagService.GetSubscribeTags(userSubscribeDto);
        }
    }
}