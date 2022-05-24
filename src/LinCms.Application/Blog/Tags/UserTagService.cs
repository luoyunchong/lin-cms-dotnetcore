using System;
using System.Threading.Tasks;
using LinCms.Aop.Attributes;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.IRepositories;

namespace LinCms.Blog.Tags
{
    public class UserTagService : ApplicationService, IUserTagService
    {
        private readonly IAuditBaseRepository<Tag> _tagRepository;
        private readonly IAuditBaseRepository<UserTag> _userTagRepository;
        private readonly ITagService _tagService;

        public UserTagService(ITagService tagService, IAuditBaseRepository<Tag> tagRepository, IAuditBaseRepository<UserTag> userTagRepository)
        {
            _tagService = tagService;
            _tagRepository = tagRepository;
            _userTagRepository = userTagRepository;
        }

        [Transactional]
        public async Task CreateUserTagAsync(Guid tagId)
        {
            Tag tag = await _tagRepository.Select.Where(r => r.Id == tagId).ToOneAsync();
            if (tag == null)
            {
                throw new LinCmsException("该标签不存在");
            }

            if (!tag.Status)
            {
                throw new LinCmsException("该标签已被拉黑");
            }

            bool any = await _userTagRepository.Select.AnyAsync(r => r.CreateUserId == CurrentUser.Id && r.TagId == tagId);
            if (any)
            {
                throw new LinCmsException("您已关注该标签");
            }

            UserTag userTag = new() { TagId = tagId };
            await _userTagRepository.InsertAsync(userTag);
            await _tagService.UpdateSubscribersCountAsync(tagId, 1);
        }

        [Transactional]
        public async Task DeleteUserTagAsync(Guid tagId)
        {
            bool any = await _userTagRepository.Select.AnyAsync(r => r.CreateUserId == CurrentUser.Id && r.TagId == tagId);
            if (!any)
            {
                throw new LinCmsException("已取消关注");
            }
            await _userTagRepository.DeleteAsync(r => r.TagId == tagId && r.CreateUserId == CurrentUser.Id);
            await _tagService.UpdateSubscribersCountAsync(tagId, -1);
        }
    }
}