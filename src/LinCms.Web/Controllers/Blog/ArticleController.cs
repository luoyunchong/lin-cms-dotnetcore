using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using LinCms.Aop.Filter;
using LinCms.Blog.Articles;
using LinCms.Blog.Classifys;
using LinCms.Data;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog
{
    [Area("blog")]
    [Route("api/blog/articles")]
    [ApiController]
    [Authorize]
    public class ArticleController : ControllerBase
    {
        private readonly IAuditBaseRepository<Article> _articleRepository;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ArticleController(IAuditBaseRepository<Article> articleRepository, IMapper mapper, ICurrentUser currentUser, IArticleService articleService)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _articleService = articleService;
        }

        /// <summary>
        /// 删除自己的随笔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<UnifyResponseDto> DeleteMyArticleAsync(Guid id)
        {
            bool isCreateArticle = _articleRepository.Select.Any(r => r.Id == id && r.CreateUserId == _currentUser.Id);
            if (!isCreateArticle)
            {
                throw new LinCmsException("无法删除别人的随笔!");
            }

            await _articleService.DeleteAsync(id);
            return UnifyResponseDto.Success();
        }

        /// <summary>
        /// 管理员删除违规随笔
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("cms/{id}")]
        [LinCmsAuthorize("删除随笔", "随笔")]
        public async Task<UnifyResponseDto> Delete(Guid id)
        {
            await _articleService.DeleteAsync(id);
            return UnifyResponseDto.Success();
        }

        /// <summary>
        /// 我所有的随笔
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public PagedResultDto<ArticleListDto> Get([FromQuery] ArticleSearchDto searchDto)
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
        public Task<PagedResultDto<ArticleListDto>> GetArticleAsync([FromQuery] ArticleSearchDto searchDto)
        {
            return _articleService.GetArticleAsync(searchDto);

            //string redisKey = "article:query:" + EncryptUtil.Encrypt(JsonConvert.SerializeObject(searchDto, Formatting.Indented, new JsonSerializerSettings
            //{
            //    DefaultValueHandling = DefaultValueHandling.Ignore
            //}));

            //return RedisHelper.CacheShellAsync(
            //    redisKey, 60, () => _articleService.GetArticleAsync(searchDto)
            // );
        }

        /// <summary>
        /// 得到所有的随笔
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [LinCmsAuthorize("所有随笔", "随笔")]
        public PagedResultDto<ArticleListDto> GetAllArticles([FromQuery] ArticleSearchDto searchDto)
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
        public async Task<ArticleDto> GetAsync(Guid id)
        {
            await _articleRepository.UpdateDiy.Set(r => r.ViewHits + 1).Where(r => r.Id == id).ExecuteAffrowsAsync();
            return await _articleService.GetAsync(id);
        }

        [HttpPost]
        public async Task<Guid> CreateAsync([FromBody] CreateUpdateArticleDto createArticle)
        {
            Guid id = await _articleService.CreateAsync(createArticle);
            return id;
        }

        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync([FromServices] IClassifyService _classifyService, Guid id, [FromBody] CreateUpdateArticleDto updateArticleDto)
        {
            await _articleService.UpdateAsync(id, updateArticleDto);
            await _articleService.UpdateTagAsync(id, updateArticleDto);
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
        public Task<PagedResultDto<ArticleListDto>> GetSubscribeArticleAsync([FromQuery] PageDto pageDto)
        {
            return _articleService.GetSubscribeArticleAsync(pageDto);
        }

        /// <summary>
        /// 修改随笔是否验证评论
        /// </summary>
        /// <param name="id">随笔主键</param>
        /// <param name="commetable">true:允许评论;false:不允许评论</param>
        /// <returns></returns>
        [HttpPut("{id}/comment-able/{commentable}")]
        public Task UpdateCommentable(Guid id, bool commetable)
        {
            return _articleService.UpdateCommentable(id, commetable);
        }
    }
}