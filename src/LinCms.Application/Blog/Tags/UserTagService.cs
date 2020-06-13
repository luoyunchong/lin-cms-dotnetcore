using System;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Core.Aop.Attributes;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Application.Blog.Tags
{
    public class UserTagService:IUserTagService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IAuditBaseRepository<Tag> _tagRepository;
        private readonly IAuditBaseRepository<UserTag> _userTagRepository;
        private readonly ITagService _tagService;

        public UserTagService(ICurrentUser currentUser, ITagService tagService, IAuditBaseRepository<Tag> tagRepository, IAuditBaseRepository<UserTag> userTagRepository)
        {
            _currentUser = currentUser;
            _tagService = tagService;
            _tagRepository = tagRepository;
            _userTagRepository = userTagRepository;
        }

        public async Task CreateUserTagAsync(Guid tagId)
        {
            Tag tag =await _tagRepository.Select.Where(r => r.Id == tagId).ToOneAsync();
            if (tag == null)
            {
                throw new LinCmsException("该标签不存在");
            }

            if (!tag.Status)
            {
                throw new LinCmsException("该标签已被拉黑");
            }

            bool any =await _userTagRepository.Select.AnyAsync(r =>
                r.CreateUserId == _currentUser.Id && r.TagId == tagId);
            if (any)
            {
                throw new LinCmsException("您已关注该标签");
            }

            UserTag userTag = new UserTag() { TagId = tagId };
            await _userTagRepository.InsertAsync(userTag);

            await _tagService.UpdateSubscribersCountAsync(tagId, 1);
        }

        [Transactional]
        public async Task DeleteUserTagAsync(Guid tagId)
        {
            bool any =await _userTagRepository.Select.AnyAsync(r => r.CreateUserId == _currentUser.Id && r.TagId == tagId);
            if (!any)
            {
                    throw new LinCmsException("已取消关注");
            }
            await _userTagRepository.DeleteAsync(r => r.TagId == tagId && r.CreateUserId == _currentUser.Id);
            await _tagService.UpdateSubscribersCountAsync(tagId, -1);
        }
    }
}