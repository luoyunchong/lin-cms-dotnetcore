using System;
using LinCms.Application.Blog.Tags;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Application.Contracts.Blog.Tags.Dtos;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Application.Contracts.Blog.UserSubscribes.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinCms.Core.IRepositories;

namespace LinCms.Web.Controllers.Blog
{
    [Route("v1/user-tag")]
    [ApiController]
    [Authorize]
    public class UserTagController : ControllerBase
    {
        private readonly ICurrentUser _currentUser;
        private readonly IAuditBaseRepository<Tag> _tagRepository;
        private readonly IAuditBaseRepository<UserTag> _userTagRepository;
        private readonly ITagService _tagService;

        public UserTagController(ICurrentUser currentUser,
            IAuditBaseRepository<Tag> tagRepository,
            IAuditBaseRepository<UserTag> userTagRepository, ITagService tagService)
        {
            _currentUser = currentUser;
            _tagRepository = tagRepository;
            _userTagRepository = userTagRepository;
            _tagService = tagService;
        }

        /// <summary>
        /// 用户关注标签
        /// </summary>
        /// <param name="tagId"></param>
        [HttpPost("{tagId}")]
        public void Post(Guid tagId)
        {
            Tag tag = _tagRepository.Select.Where(r => r.Id == tagId).ToOne();
            if (tag == null)
            {
                throw new LinCmsException("该标签不存在");
            }

            if (!tag.Status)
            {
                throw new LinCmsException("该标签已被拉黑");
            }

            bool any = _userTagRepository.Select.Any(r =>
                  r.CreateUserId == _currentUser.Id && r.TagId == tagId);
            if (any)
            {
                throw new LinCmsException("您已关注该标签");
            }

            UserTag userTag = new UserTag() { TagId = tagId };
            _userTagRepository.Insert(userTag);

            _tagService.UpdateSubscribersCount(tagId, 1);
        }

        /// <summary>
        /// 取消关注标签
        /// </summary>
        /// <param name="tagId"></param>
        [HttpDelete("{tagId}")]
        public void Delete(Guid tagId)
        {
            bool any = _userTagRepository.Select.Any(r => r.CreateUserId == _currentUser.Id && r.TagId == tagId);
            if (!any)
            {
                throw new LinCmsException("已取消关注");
            }
            _userTagRepository.Delete(r => r.TagId == tagId && r.CreateUserId == _currentUser.Id);
            _tagService.UpdateSubscribersCount(tagId, -1);
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