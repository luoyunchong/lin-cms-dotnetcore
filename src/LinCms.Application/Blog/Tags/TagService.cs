using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Blog.UserSubscribes;
using LinCms.Data.Enums;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Blog.Tags;

/// <summary>
/// 标签服务
/// </summary>
public class TagService(IAuditBaseRepository<Tag> tagRepository, IAuditBaseRepository<UserTag> userTagRepository,
        IAuditBaseRepository<TagArticle> tagArticleRepository, IFileRepository fileRepository)
    : ApplicationService, ITagService
{
    #region CRUD
    public async Task CreateAsync(CreateUpdateTagDto createTag)
    {
        bool exist = await tagRepository.Select.AnyAsync(r => r.TagName == createTag.TagName);
        if (exist)
        {
            throw new LinCmsException($"标签[{createTag.TagName}]已存在");
        }

        Tag tag = Mapper.Map<Tag>(createTag);
        await tagRepository.InsertAsync(tag);
    }

    public async Task UpdateAsync(Guid id, CreateUpdateTagDto updateTag)
    {
        Tag tag = await tagRepository.Select.Where(r => r.Id == id).ToOneAsync();
        if (tag == null)
        {
            throw new LinCmsException("该数据不存在");
        }

        bool exist = await tagRepository.Select.AnyAsync(r => r.TagName == updateTag.TagName && r.Id != id);
        if (exist)
        {
            throw new LinCmsException($"标签[{updateTag.TagName}]已存在");
        }

        Mapper.Map(updateTag, tag);
        await tagRepository.UpdateAsync(tag);
    }

    public async Task<TagListDto> GetAsync(Guid id)
    {
        Tag tag = await tagRepository.Select.Where(a => a.Id == id).ToOneAsync();
        if (tag == null)
        {
            throw new LinCmsException("不存在此标签");
        }
        TagListDto tagDto = Mapper.Map<TagListDto>(tag);
        tagDto.IsSubscribe = await IsSubscribeAsync(id);
        tagDto.ThumbnailDisplay = fileRepository.GetFileUrl(tagDto.Thumbnail);
        return tagDto;
    }

    /// <summary>
    /// 根据状态得到标签列表
    /// </summary>
    /// <param name="searchDto"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<TagListDto>> GetListAsync(TagSearchDto searchDto)
    {
        if (searchDto.Sort.IsNullOrEmpty())
        {
            searchDto.Sort = "create_time desc";
        }

        List<TagListDto> tags = (await tagRepository.Select.IncludeMany(r => r.UserTags, r => r.Where(u => u.CreateUserId == CurrentUser.FindUserId()))
                .WhereIf(searchDto.TagIds.IsNotNullOrEmpty(), r => searchDto.TagIds.Contains(r.Id))
                .WhereIf(searchDto.TagName.IsNotNullOrEmpty(), r => r.TagName.Contains(searchDto.TagName))
                .WhereIf(searchDto.Status != null, r => r.Status == searchDto.Status)
                .OrderBy(searchDto.Sort)
                .ToPagerListAsync(searchDto, out long totalCount))
            .Select(r =>
            {
                TagListDto tagDto = Mapper.Map<TagListDto>(r);
                tagDto.ThumbnailDisplay = fileRepository.GetFileUrl(tagDto.Thumbnail);
                tagDto.IsSubscribe = r.UserTags.Any();
                return tagDto;
            }).ToList();

        return new PagedResultDto<TagListDto>(tags, totalCount);
    }

    #endregion

    public async Task<bool> IsSubscribeAsync(Guid tagId)
    {
        if (CurrentUser.FindUserId() == null) return false;
        return await userTagRepository.Select.AnyAsync(r => r.TagId == tagId && r.CreateUserId == CurrentUser.FindUserId());
    }

    public PagedResultDto<TagListDto> GetSubscribeTags(UserSubscribeSearchDto userSubscribeDto)
    {
        List<Guid> userTagIds = userTagRepository.Select
            .Where(u => u.CreateUserId == CurrentUser.FindUserId())
            .ToList(r => r.TagId);

        List<TagListDto> tagListDtos = userTagRepository.Select.Include(r => r.Tag)
            .Where(r => r.CreateUserId == userSubscribeDto.UserId)
            .OrderByDescending(r => r.CreateTime)
            .ToPagerList(userSubscribeDto, out long count)
            .Select(r =>
            {
                TagListDto tagDto = Mapper.Map<TagListDto>(r.Tag);
                if (tagDto != null)
                {
                    tagDto.ThumbnailDisplay = fileRepository.GetFileUrl(tagDto.Thumbnail);
                    tagDto.IsSubscribe = userTagIds.Any(tagId => tagId == tagDto.Id);
                }
                else
                {
                    return new TagListDto()
                    {
                        Id = r.TagId,
                        TagName = "该标签已被拉黑",
                        IsSubscribe = userTagIds.Any(tagId => tagId == r.TagId)
                    };
                }
                return tagDto;
            }).ToList();

        return new PagedResultDto<TagListDto>(tagListDtos, count);
    }

    public async Task UpdateArticleCountAsync(Guid? id, int inCreaseCount)
    {
        if (id == null)
        {
            return;
        }
        Tag tag = await tagRepository.Select.Where(r => r.Id == id).ToOneAsync();
        //防止数量一直减，减到小于0
        if (inCreaseCount < 0)
        {
            if (tag.ArticleCount < -inCreaseCount)
            {
                return;
            }
        }
        tag.ArticleCount = tag.ArticleCount + inCreaseCount;

        await tagRepository.UpdateAsync(tag);
    }


    public async Task UpdateSubscribersCountAsync(Guid? id, int inCreaseCount)
    {
        if (id == null)
        {
            return;
        }

        Tag tag = await tagRepository.Select.Where(r => r.Id == id).ToOneAsync();
        if (tag == null)
        {
            throw new LinCmsException("标签不存在", ErrorCode.NotFound);
        }

        tag.UpdateSubscribersCount(inCreaseCount);
        await tagRepository.UpdateAsync(tag);
    }
    public async Task CorrectedTagCountAsync(Guid tagId)
    {
        long count = await tagArticleRepository.Select.Where(r => r.TagId == tagId && r.Article.IsDeleted == false).CountAsync();
        await tagRepository.UpdateDiy.Set(r => r.ArticleCount, count).Where(r => r.Id == tagId).ExecuteAffrowsAsync();
    }

    public async Task IncreaseTagViewHits(Guid tagId)
    {
        await tagRepository.UpdateDiy.Set(r => r.ViewHits + 1)
            .Where(r => r.Id == tagId)
            .ExecuteAffrowsAsync();
    }
}