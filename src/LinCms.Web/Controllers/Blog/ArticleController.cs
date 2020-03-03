using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.CAP;
using FreeSql;
using LinCms.Application.Blog.Articles;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    [Route("v1/article")]
    [ApiController]
    [Authorize]
    public class ArticleController : ControllerBase
    {
        private readonly AuditBaseRepository<Article> _articleRepository;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly ICapPublisher _capBus;
        public ArticleController(AuditBaseRepository<Article> articleRepository,
            IMapper mapper,
            ICurrentUser currentUser,
            IArticleService articleService,
            ICapPublisher capBus)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _articleService = articleService;
            _capBus = capBus;
        }
        /// <summary>
        /// 删除自己的随笔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public UnifyResponseDto DeleteMyArticle(Guid id)
        {
            bool isCreateArticle = _articleRepository.Select.Any(r => r.Id == id && r.CreateUserId == _currentUser.Id);
            if (!isCreateArticle)
            {
                throw new LinCmsException("无法删除别人的随笔!");
            }
            _articleService.Delete(id);
            return UnifyResponseDto.Success();
        }

        /// <summary>
        /// 管理员删除违规随笔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("cms/{id}")]
        [LinCmsAuthorize("删除随笔", "随笔")]
        public UnifyResponseDto Delete(Guid id)
        {
            _articleService.Delete(id);
            return UnifyResponseDto.Success();
        }

        /// <summary>
        /// 我所有的随笔
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public PagedResultDto<ArticleListDto> Get([FromQuery]ArticleSearchDto searchDto)
        {
            List<ArticleListDto> articles = _articleRepository
                .Select
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                .Where(r => r.CreateUserId == _currentUser.Id)
                .WhereIf(searchDto.Title.IsNotNullOrEmpty(), r => r.Title.Contains(searchDto.Title))
                .WhereIf(searchDto.ClassifyId.HasValue, r => r.ClassifyId == searchDto.ClassifyId)
                .OrderByDescending(r => r.IsStickie)
                .OrderByDescending(r => r.Id)
                .ToPagerList(searchDto, out long totalCount)
                .Select(a => _mapper.Map<ArticleListDto>(a))
                .ToList();

            return new PagedResultDto<ArticleListDto>(articles, totalCount);
        }

        /// <summary>
        /// 得到所有已审核过的随笔,最新的随笔/三天、七天、月榜、全部
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet("query")]
        [AllowAnonymous]
        public async Task<PagedResultDto<ArticleListDto>> GetArticleAsync([FromQuery]ArticleSearchDto searchDto)
        {
            return await _articleService.GetArticleAsync(searchDto);
            //string redisKey = "article:query:" + searchDto.ToString();
            //return await RedisHelper.CacheShellAsync(
            //    redisKey, 3600, async () => await _articleService.GetArticleAsync(searchDto)
            //   );
        }

        /// <summary>
        /// 得到所有已审核过的随笔
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [LinCmsAuthorize("所有随笔", "随笔")]
        public PagedResultDto<ArticleListDto> GetAllArticles([FromQuery]ArticleSearchDto searchDto)
        {
            var articles = _articleRepository
                .Select
                .WhereCascade(r => r.IsDeleted == false)
                .WhereIf(searchDto.Title.IsNotNullOrEmpty(), r => r.Title.Contains(searchDto.Title))
                .OrderByDescending(r => r.CreateTime)
                .ToPagerList(searchDto, out long totalCount)
                .Select(a => _mapper.Map<ArticleListDto>(a))
                .ToList();

            return new PagedResultDto<ArticleListDto>(articles, totalCount);
        }

        /// <summary>
        /// 随笔详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public Task<ArticleDto> GetAsync(Guid id)
        {
            _articleRepository.UpdateDiy.Set(r => r.ViewHits + 1).Where(r => r.Id == id).ExecuteAffrows();
            return _articleService.GetAsync(id);
        }

        [HttpPost]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateArticleDto createArticle)
        {
            await _articleService.CreateAsync(createArticle);

            _capBus.Publish("NotificationController.Post", new CreateNotificationDto()
            {

            });

            return UnifyResponseDto.Success("新建随笔成功");
        }


        [HttpPut("{id}")]
        public UnifyResponseDto UpdateAsync(Guid id, [FromBody] CreateUpdateArticleDto updateArticle)
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
            _articleService.UpdateAsync(updateArticle, article);

            return UnifyResponseDto.Success("更新随笔成功");
        }

        /// <summary>
        /// 审核随笔-拉黑/取消拉黑
        /// </summary>
        /// <param name="id">审论Id</param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        [LinCmsAuthorize("审核随笔", "随笔")]
        [HttpPut("audit/{id}")]
        public async Task<UnifyResponseDto> AuditAsync(Guid id, bool isAudit)
        {
            Article article = await _articleRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (article == null)
            {
                throw new LinCmsException("没有找到相关随笔");
            }

            article.IsAudit = isAudit;
            await _articleRepository.UpdateAsync(article);
            return UnifyResponseDto.Success();
        }

        /// <summary>
        /// 得到我关注的人发布的随笔
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        [HttpGet("subscribe")]
        public PagedResultDto<ArticleListDto> GetSubscribeArticle([FromQuery]PageDto pageDto)
        {
            return _articleService.GetSubscribeArticle(pageDto);
        }
    }
}