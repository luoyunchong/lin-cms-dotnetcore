using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Blog.Classifys;
using LinCms.Application.Contracts.Blog.Classifys.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Aop.Filter;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Mvc;
using LinCms.Core.IRepositories;
using LinCms.Web.Data.Authorization;

namespace LinCms.Web.Controllers.Blog
{
    [Route("v1/classify")]
    [ApiController]
    public class ClassifyController : ControllerBase
    {
        private readonly IAuditBaseRepository<Classify> _classifyRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public readonly IFileRepository _fileRepository;
        public ClassifyController(IAuditBaseRepository<Classify> classifyRepository, IMapper mapper, ICurrentUser currentUser, IFileRepository fileRepository)
        {
            _classifyRepository = classifyRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _fileRepository = fileRepository;
        }

        [HttpDelete("{id}")]
        public async Task<UnifyResponseDto> DeleteClassify(Guid id)
        {
            Classify classify = await _classifyRepository.Select.Where(a => a.Id == id).ToOneAsync();
            if (classify.CreateUserId != _currentUser.Id)
            {
                throw new LinCmsException("您无权删除他人的分类专栏");
            }
            await _classifyRepository.DeleteAsync(new Classify { Id = id });
            return UnifyResponseDto.Success();
        }

        [HttpGet]
        public List<ClassifyDto> Get(long? userId)
        {
            if (!userId.HasValue)
            {
                userId = _currentUser.Id;
            }
            List<ClassifyDto> classify = _classifyRepository.Select.OrderBy(r => r.SortCode)
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

        [LinCmsAuthorize("删除", "分类专栏")]
        [HttpDelete("cms/{id}")]
        public UnifyResponseDto Delete(Guid id)
        {
            _classifyRepository.Delete(new Classify { Id = id });
            return UnifyResponseDto.Success();
        }

        [LinCmsAuthorize("分类专栏列表", "分类专栏")]
        [HttpGet("cms")]
        public PagedResultDto<ClassifyDto> Get([FromQuery] ClassifySearchDto pageDto)
        {
            List<ClassifyDto> classify = _classifyRepository.Select
                .WhereIf(pageDto.ClassifyName.IsNotNullOrEmpty(), r => r.ClassifyName.Contains(pageDto.ClassifyName))
                .OrderByDescending(r => r.CreateTime)
                .ToPagerList(pageDto, out long totalCount)
                .Select(r =>
                {
                    ClassifyDto classifyDto = _mapper.Map<ClassifyDto>(r);
                    classifyDto.ThumbnailDisplay = _fileRepository.GetFileUrl(classifyDto.Thumbnail);
                    return classifyDto;
                }).ToList();

            return new PagedResultDto<ClassifyDto>(classify, totalCount);
        }

        [HttpGet("{id}")]
        public async Task<ClassifyDto> GetAsync(Guid id)
        {
            Classify classify = await _classifyRepository.Select.Where(a => a.Id == id).ToOneAsync();
            ClassifyDto classifyDto = _mapper.Map<ClassifyDto>(classify);
            classifyDto.ThumbnailDisplay = _fileRepository.GetFileUrl(classifyDto.Thumbnail);
            return classifyDto;
        }

        [HttpPost]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateClassifyDto createClassify)
        {
            bool exist = await _classifyRepository.Select.AnyAsync(r => r.ClassifyName == createClassify.ClassifyName && r.CreateUserId == _currentUser.Id);
            if (exist)
            {
                throw new LinCmsException($"分类专栏[{createClassify.ClassifyName}]已存在");
            }

            Classify classify = _mapper.Map<Classify>(createClassify);
            await _classifyRepository.InsertAsync(classify);
            return UnifyResponseDto.Success("新建分类专栏成功");
        }

        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateClassifyDto updateClassify)
        {
            Classify classify = await _classifyRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (classify == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            if (classify.CreateUserId != _currentUser.Id)
            {
                throw new LinCmsException("您无权编辑他人的分类专栏");
            }

            bool exist = await _classifyRepository.Select.AnyAsync(r => r.ClassifyName == updateClassify.ClassifyName && r.Id != id && r.CreateUserId == _currentUser.Id);
            if (exist)
            {
                throw new LinCmsException($"分类专栏[{updateClassify.ClassifyName}]已存在");
            }

            _mapper.Map(updateClassify, classify);

            await _classifyRepository.UpdateAsync(classify);
            return UnifyResponseDto.Success("更新分类专栏成功");
        }

    }
}