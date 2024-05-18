using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Security;

namespace LinCms.Blog.Tags;

public class UserTagService(ITagService tagService, IAuditBaseRepository<Tag> tagRepository,
        IAuditBaseRepository<UserTag> userTagRepository)
    : ApplicationService, IUserTagService
{
    [Transactional]
    public async Task CreateUserTagAsync(Guid tagId)
    {
        Tag tag = await tagRepository.Select.Where(r => r.Id == tagId).ToOneAsync();
        if (tag == null)
        {
            throw new LinCmsException("该标签不存在");
        }

        if (!tag.Status)
        {
            throw new LinCmsException("该标签已被拉黑");
        }

        bool any = await userTagRepository.Select.AnyAsync(r => r.CreateUserId ==  CurrentUser.FindUserId() && r.TagId == tagId);
        if (any)
        {
            throw new LinCmsException("您已关注该标签");
        }

        UserTag userTag = new() { TagId = tagId };
        await userTagRepository.InsertAsync(userTag);
        await tagService.UpdateSubscribersCountAsync(tagId, 1);
    }

    [Transactional]
    public async Task DeleteUserTagAsync(Guid tagId)
    {
        bool any = await userTagRepository.Select.AnyAsync(r => r.CreateUserId ==  CurrentUser.FindUserId() && r.TagId == tagId);
        if (!any)
        {
            throw new LinCmsException("已取消关注");
        }
        await userTagRepository.DeleteAsync(r => r.TagId == tagId && r.CreateUserId ==  CurrentUser.FindUserId());
        await tagService.UpdateSubscribersCountAsync(tagId, -1);
    }
}