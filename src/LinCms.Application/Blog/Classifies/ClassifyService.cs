using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Blog.Classifys;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Blog.Classifies;

public class ClassifyService(IAuditBaseRepository<Classify, Guid> repository, IFileRepository fileRepository)
    : CrudAppService<Classify, ClassifyDto, ClassifyDto, Guid, ClassifySearchDto, CreateUpdateClassifyDto,
        CreateUpdateClassifyDto>(repository), IClassifyService
{
    private readonly IFileRepository _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));

    #region CRUD

    public override async Task<PagedResultDto<ClassifyDto>> GetListAsync(ClassifySearchDto input)
    {
        List<ClassifyDto> classify = (await Repository.Select
                .WhereIf(input.ClassifyName.IsNotNullOrEmpty(), r => r.ClassifyName.Contains(input.ClassifyName))
                .OrderByDescending(r => r.CreateTime)
                .ToPagerListAsync(input, out long totalCount))
            .Select(r =>
            {
                ClassifyDto classifyDto = Mapper.Map<ClassifyDto>(r);
                classifyDto.ThumbnailDisplay = _fileRepository.GetFileUrl(classifyDto.Thumbnail);
                return classifyDto;
            }).ToList();

        return new PagedResultDto<ClassifyDto>(classify, totalCount);
    }

    public List<ClassifyDto> GetListByUserId(long? userId)
    {
        if (!userId.HasValue)
        {
            userId = CurrentUser.FindUserId();
        }

        List<ClassifyDto> classify = Repository.Select
            .Where(r => r.CreateUserId == userId)
            .OrderByDescending(r => r.SortCode)
            .ToList()
            .Select(r =>
            {
                ClassifyDto classifyDto = Mapper.Map<ClassifyDto>(r);
                classifyDto.ThumbnailDisplay = _fileRepository.GetFileUrl(classifyDto.Thumbnail);
                return classifyDto;
            }).ToList();

        return classify;
    }

    public override async Task<ClassifyDto> GetAsync(Guid id)
    {
        Classify classify = await Repository.Select.Where(a => a.Id == id).ToOneAsync();
        ClassifyDto classifyDto = Mapper.Map<ClassifyDto>(classify);
        classifyDto.ThumbnailDisplay = _fileRepository.GetFileUrl(classifyDto.Thumbnail);
        return classifyDto;
    }

    public override async Task<ClassifyDto> CreateAsync(CreateUpdateClassifyDto createClassify)
    {
        bool exist = await Repository.Select.AnyAsync(r =>
            r.ClassifyName == createClassify.ClassifyName && r.CreateUserId == CurrentUser.FindUserId());
        if (exist)
        {
            throw new LinCmsException($"分类专栏[{createClassify.ClassifyName}]已存在");
        }

        Classify classify = Mapper.Map<Classify>(createClassify);
        await Repository.InsertAsync(classify);
        return Mapper.Map<ClassifyDto>(classify);
    }

    public override async Task<ClassifyDto> UpdateAsync(Guid id, CreateUpdateClassifyDto updateInput)
    {
        Classify classify = await Repository.Select.Where(r => r.Id == id).ToOneAsync();
        if (classify == null)
        {
            throw new LinCmsException("该数据不存在");
        }

        if (classify.CreateUserId != CurrentUser.FindUserId())
        {
            throw new LinCmsException("您无权编辑他人的分类专栏");
        }

        bool exist = await Repository.Select.AnyAsync(r => r.ClassifyName == updateInput.ClassifyName && r.Id != id && r.CreateUserId == CurrentUser.FindUserId());
        if (exist)
        {
            throw new LinCmsException($"分类专栏[{updateInput.ClassifyName}]已存在");
        }

        Mapper.Map(updateInput, classify);

        await Repository.UpdateAsync(classify);
        return Mapper.Map<ClassifyDto>(classify);
    }

    #endregion

    public override async Task DeleteAsync(Guid id)
    {
        Classify classify = await Repository.Select.Where(a => a.Id == id).ToOneAsync();
        if (classify.CreateUserId != CurrentUser.FindUserId())
        {
            throw new LinCmsException("您无权删除他人的分类专栏");
        }

        await Repository.DeleteAsync(new Classify { Id = id });
    }

    public async Task UpdateArticleCountAsync(Guid? id, int inCreaseCount)
    {
        if (id == null)
        {
            return;
        }
        Classify classify = await Repository.Select.Where(r => r.Id == id).ToOneAsync();
        classify.UpdateArticleCount(inCreaseCount);
        await Repository.UpdateAsync(classify);
    }
}