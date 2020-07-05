using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using LinCms.Application.Contracts.Blog.Classifys;
using LinCms.Application.Contracts.Blog.Classifys.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Application.Blog.Classifies
{
    public class ClassifyService : CrudAppService<Classify, ClassifyDto, ClassifyDto, Guid, ClassifySearchDto, CreateUpdateClassifyDto,CreateUpdateClassifyDto>, IClassifyService
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IFileRepository _fileRepository;

        public ClassifyService(IAuditBaseRepository<Classify, Guid> classifyBaseRepository, IMapper mapper,
            ICurrentUser currentUser, IFileRepository fileRepository) : base(classifyBaseRepository)
        {
            _mapper = mapper;
            _currentUser = currentUser;
            _fileRepository = fileRepository;
        }
        
        public override async Task<PagedResultDto<ClassifyDto>> GetListAsync(ClassifySearchDto searchDto)
        {
            List<ClassifyDto> classify = (await Repository.Select
                    .WhereIf(searchDto.ClassifyName.IsNotNullOrEmpty(),
                        r => r.ClassifyName.Contains(searchDto.ClassifyName))
                    .OrderByDescending(r => r.CreateTime)
                    .ToPagerListAsync(searchDto, out long totalCount))
                .Select(r =>
                {
                    ClassifyDto classifyDto = _mapper.Map<ClassifyDto>(r);
                    classifyDto.ThumbnailDisplay = _fileRepository.GetFileUrl(classifyDto.Thumbnail);
                    return classifyDto;
                }).ToList();

            return new PagedResultDto<ClassifyDto>(classify, totalCount);
        }

        public  List<ClassifyDto> GetListByUserId(long? userId)
        {
            if (!userId.HasValue)
            {
                userId = _currentUser.Id;
            }

            List<ClassifyDto> classify = Repository.Select
                .Where(r => r.CreateUserId == userId)
                .OrderByDescending(r => r.SortCode)
                .ToList()
                .Select(r =>
                {
                    ClassifyDto classifyDto = _mapper.Map<ClassifyDto>(r);
                    classifyDto.ThumbnailDisplay = _fileRepository.GetFileUrl(classifyDto.Thumbnail);
                    return classifyDto;
                }).ToList();

            return classify;
        }

        public override async Task<ClassifyDto> GetAsync(Guid id)
        {
            Classify classify = await Repository.Select.Where(a => a.Id == id).ToOneAsync();
            ClassifyDto classifyDto = _mapper.Map<ClassifyDto>(classify);
            classifyDto.ThumbnailDisplay = _fileRepository.GetFileUrl(classifyDto.Thumbnail);
            return classifyDto;
        }

        public override async Task<ClassifyDto> CreateAsync(CreateUpdateClassifyDto createClassify)
        {
            bool exist = await Repository.Select.AnyAsync(r =>
                r.ClassifyName == createClassify.ClassifyName && r.CreateUserId == _currentUser.Id);
            if (exist)
            {
                throw new LinCmsException($"分类专栏[{createClassify.ClassifyName}]已存在");
            }

            Classify classify = _mapper.Map<Classify>(createClassify);
            await Repository.InsertAsync(classify);
            return _mapper.Map<ClassifyDto>(classify);
        }

        public override async Task<ClassifyDto> UpdateAsync(Guid id, CreateUpdateClassifyDto updateClassify)
        {
            Classify classify = await Repository.Select.Where(r => r.Id == id).ToOneAsync();
            if (classify == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            if (classify.CreateUserId != _currentUser.Id)
            {
                throw new LinCmsException("您无权编辑他人的分类专栏");
            }

            bool exist = await Repository.Select.AnyAsync(r =>
                r.ClassifyName == updateClassify.ClassifyName && r.Id != id && r.CreateUserId == _currentUser.Id);
            if (exist)
            {
                throw new LinCmsException($"分类专栏[{updateClassify.ClassifyName}]已存在");
            }

            _mapper.Map(updateClassify, classify);

            await Repository.UpdateAsync(classify);
            return _mapper.Map<ClassifyDto>(classify);
        }

        public override async Task DeleteAsync(Guid id)
        {
            Classify classify = await Repository.Select.Where(a => a.Id == id).ToOneAsync();
            if (classify.CreateUserId != _currentUser.Id)
            {
                throw new LinCmsException("您无权删除他人的分类专栏");
            }

            await Repository.DeleteAsync(new Classify {Id = id});
        }

        public async Task UpdateArticleCountAsync(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }

            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                Classify classify = await Repository.Select.Where(r => r.Id == id).ToOneAsync();
                if (classify.ArticleCount < -inCreaseCount)
                {
                    return;
                }
            }

            await Repository.UpdateDiy.Set(r => r.ArticleCount + inCreaseCount).Where(r => r.Id == id)
                .ExecuteAffrowsAsync();
        }
    }
}