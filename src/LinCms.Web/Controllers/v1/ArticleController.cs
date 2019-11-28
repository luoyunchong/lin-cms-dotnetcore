using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.v1.Articles;
using LinCms.Web.Models.v1.Tags;
using LinCms.Web.Services.v1.Interfaces;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/article")]
    [ApiController]
    [Authorize]
    public class ArticleController : ControllerBase
    {
        private readonly AuditBaseRepository<Article> _articleRepository;
        private readonly GuidRepository<TagArticle> _tagArticleRepository;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public ArticleController(AuditBaseRepository<Article> articleRepository, IMapper mapper, ICurrentUser currentUser, 
            GuidRepository<TagArticle> tagArticleRepository, IArticleService articleService)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _tagArticleRepository = tagArticleRepository;
            _articleService = articleService;
        }

        [HttpDelete("{id}")]
        public ResultDto DeleteMyArticle(Guid id)
        {
            bool isCreateArticle = _articleRepository.Select.Any(r => r.Id == id && r.CreateUserId == _currentUser.Id);
            if (!isCreateArticle)
            {
                throw new LinCmsException("无法删除别人的随笔!");
            }
            _articleService.Delete(id);
            return ResultDto.Success();
        }

        [HttpDelete("cms/{id}")]
        [LinCmsAuthorize("删除随笔", "随笔")]
        public ResultDto Delete(Guid id)
        {
            _articleService.Delete(id);
            return ResultDto.Success();
        }

        /// <summary>
        /// 我所有的随笔
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet]
        public PagedResultDto<ArticleDto> Get([FromQuery]ArticleSearchDto searchDto)
        {
            var select = _articleRepository
                .Select
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                .Where(r => r.CreateUserId == _currentUser.Id)
                .WhereIf(searchDto.Title.IsNotNullOrEmpty(), r => r.Title.Contains(searchDto.Title))
                .WhereIf(searchDto.ClassifyId.HasValue,r=>r.ClassifyId== searchDto.ClassifyId)
                .OrderByDescending(r => r.IsStickie)
                .OrderByDescending(r => r.Id);

            var articles = select
            .ToPagerList(searchDto, out long totalCount)
            .Select(a =>
            {
                ArticleDto articleDto = _mapper.Map<ArticleDto>(a);
                return articleDto;
            })
            .ToList();

            return new PagedResultDto<ArticleDto>(articles, totalCount);
        }

        /// <summary>
        /// 得到所有的随笔
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [AllowAnonymous]
        public PagedResultDto<ArticleDto> GetLastArticles([FromQuery]ArticleSearchDto searchDto)
        {
            long? userId = _currentUser.Id;
            var select = _articleRepository
                .Select
                .Include(r => r.Classify)
                .IncludeMany(r => r.Tags,r=>r.Where(u=>u.Status==true))
                .IncludeMany(r => r.UserLikes,r=>r.Where(u => u.CreateUserId == userId))
                .Where(r=>r.IsAudit==true)
                .WhereIf(searchDto.TagId.HasValue,r=>r.Tags.AsSelect().Any(u=>u.Id==searchDto.TagId))
                .WhereIf(searchDto.ClassifyId.HasValue, r => r.ClassifyId == searchDto.ClassifyId)
                .WhereIf(searchDto.Title.IsNotNullOrEmpty(), r => r.Title.Contains(searchDto.Title))
                .OrderByDescending(r => r.CreateTime);

            var articles = select
                .ToPagerList(searchDto, out long totalCount)
                .Select(a =>
                {
                    ArticleDto articleDto = _mapper.Map<ArticleDto>(a);
                    articleDto.Tags = a.Tags.Select(r => new TagDto()
                    {
                        TagName = r.TagName,
                        Id = r.Id,
                    }).ToList();

                    articleDto.IsLiked = userId != null && a.UserLikes.Any();

                    articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(articleDto.Thumbnail);
                    return articleDto;
                })
                .ToList();

            return new PagedResultDto<ArticleDto>(articles, totalCount);
        }

        /// <summary>
        /// 随笔详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ArticleDto Get(Guid id)
        {
            _articleRepository.UpdateDiy.Set(r => r.ViewHits + 1).Where(r => r.Id == id).ExecuteAffrows();
            return _articleService.Get(id);
        }

        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateArticleDto createArticle)
        {
            _articleService.CreateArticle(createArticle);

            return ResultDto.Success("新建随笔成功");
        }


        [HttpPut("{id}")]
        public ResultDto Put(Guid id, [FromBody] CreateUpdateArticleDto updateArticle)
        {
            Article article = _articleRepository.Select.Where(r => r.Id == id).ToOne();
            if (article.CreateUserId != _currentUser.Id)
            {
                throw new LinCmsException("您无权修改他人的随笔");
            }
            if (article == null)
            {
                throw new LinCmsException("没有找到相关随笔");
            }

            _mapper.Map(updateArticle, article);
            _articleService.UpdateArticle(updateArticle,article);

            return ResultDto.Success("更新随笔成功");
        }

        /// <summary>
        /// 审核随笔-拉黑/取消拉黑
        /// </summary>
        /// <param name="id">审论Id</param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        [LinCmsAuthorize("审核随笔", "随笔")]
        [HttpPut("audit/{id}")]
        public ResultDto Put(Guid id, bool isAudit)
        {
            Article article = _articleRepository.Select.Where(r => r.Id == id).ToOne();
            if (article == null)
            {
                throw new LinCmsException("没有找到相关随笔");
            }

            article.IsAudit = isAudit;
            _articleRepository.Update(article);
            return ResultDto.Success();
        }
    }
}