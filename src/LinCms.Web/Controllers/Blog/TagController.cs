using System;
using AutoMapper;
using FreeSql;
using LinCms.Application.Blog.Tags;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    [Route("v1/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly AuditBaseRepository<Tag> _tagRepository;
        private readonly BaseRepository<TagArticle> _tagArticleRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly ITagService _tagService;
        public TagController(AuditBaseRepository<Tag> tagRepository, BaseRepository<TagArticle> tagArticleRepository, IMapper mapper, ICurrentUser currentUser, ITagService tagService)
        {
            _tagRepository = tagRepository;
            _tagArticleRepository = tagArticleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _tagService = tagService;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除标签", "标签管理")]
        public ResultDto DeleteTag(Guid id)
        {
            _tagRepository.Delete(new Tag { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        [LinCmsAuthorize("所有标签", "标签管理")]
        public PagedResultDto<TagDto> GetAll([FromQuery]TagSearchDto searchDto)
        {
            return _tagService.Get(searchDto);
        }

        [HttpGet("public")]
        public PagedResultDto<TagDto> Get([FromQuery]TagSearchDto searchDto)
        {
            searchDto.Status = true;
            return _tagService.Get(searchDto);
        }

        [HttpGet("{id}")]
        public TagDto Get(Guid id)
        {
            return _tagService.Get(id);
        }

        [HttpPost]
        [LinCmsAuthorize("新增标签", "标签管理")]
        public ResultDto Post([FromBody] CreateUpdateTagDto createTag)
        {
            bool exist = _tagRepository.Select.Any(r => r.TagName == createTag.TagName);
            if (exist)
            {
                throw new LinCmsException($"标签[{createTag.TagName}]已存在");
            }

            Tag tag = _mapper.Map<Tag>(createTag);
            _tagRepository.Insert(tag);
            return ResultDto.Success("新建标签成功");
        }

        [LinCmsAuthorize("编辑标签", "标签管理")]
        [HttpPut("{id}")]
        public ResultDto Put(Guid id, [FromBody] CreateUpdateTagDto updateTag)
        {
            Tag tag = _tagRepository.Select.Where(r => r.Id == id).ToOne();
            if (tag == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = _tagRepository.Select.Any(r => r.TagName == updateTag.TagName && r.Id != id);
            if (exist)
            {
                throw new LinCmsException($"标签[{updateTag.TagName}]已存在");
            }

            _mapper.Map(updateTag, tag);

            _tagRepository.Update(tag);

            return ResultDto.Success("更新标签成功");
        }

        /// <summary>
        /// 标签-校正标签对应文章数量
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        [LinCmsAuthorize("校正文章数量", "标签管理")]
        [HttpPut("correct/{tagId}")]
        public ResultDto CorrectedTagCount(Guid tagId)
        {
            long count = _tagArticleRepository.Select.Where(r => r.TagId == tagId && r.Article.IsDeleted == false).Count();
            _tagRepository.UpdateDiy.Set(r => r.ArticleCount, count).Where(r => r.Id == tagId).ExecuteAffrows();
            return ResultDto.Success();
        }
    }
}