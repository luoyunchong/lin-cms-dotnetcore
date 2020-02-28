using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly GuidRepository<TagArticle> _tagArticleRepository;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly BaseRepository<UserSubscribe> _userSubscribeRepository;
        private readonly ICapPublisher _capBus;
        public ArticleController(AuditBaseRepository<Article> articleRepository, IMapper mapper, ICurrentUser currentUser,
            GuidRepository<TagArticle> tagArticleRepository, IArticleService articleService, BaseRepository<UserSubscribe> userSubscribeRepository, ICapPublisher capBus)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _tagArticleRepository = tagArticleRepository;
            _articleService = articleService;
            _userSubscribeRepository = userSubscribeRepository;
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
        public PagedResultDto<ArticleListDto> QueryArticles([FromQuery]ArticleSearchDto searchDto)
        {
            DateTime monthDays = DateTime.Now.AddDays(-30);
            DateTime weeklyDays = DateTime.Now.AddDays(-7);
            DateTime threeDays = DateTime.Now.AddDays(-3);

            long? userId = _currentUser.Id;
            ISelect<Article> articleSelect = _articleRepository
                .Select
                .Include(r => r.Classify)
                .Include(r => r.UserInfo)
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                .IncludeMany(r => r.UserLikes, r => r.Where(u => u.CreateUserId == userId))
                .Where(r => r.IsAudit == true)
                .WhereCascade(r => r.IsDeleted == false)
                .WhereIf(searchDto.UserId != null, r => r.CreateUserId == searchDto.UserId)
                .WhereIf(searchDto.TagId.HasValue, r => r.Tags.AsSelect().Any(u => u.Id == searchDto.TagId))
                .WhereIf(searchDto.ClassifyId.HasValue, r => r.ClassifyId == searchDto.ClassifyId)
                .WhereIf(searchDto.ChannelId.HasValue, r => r.ChannelId == searchDto.ChannelId)
                .WhereIf(searchDto.Title.IsNotNullOrEmpty(), r => r.Title.Contains(searchDto.Title))
                .WhereIf(searchDto.Sort == "THREE_DAYS_HOTTEST", r => r.CreateTime > threeDays)
                .WhereIf(searchDto.Sort == "WEEKLY_HOTTEST", r => r.CreateTime > weeklyDays)
                .WhereIf(searchDto.Sort == "MONTHLY_HOTTEST", r => r.CreateTime > monthDays)
                .OrderByDescending(
                    searchDto.Sort == "THREE_DAYS_HOTTEST" || searchDto.Sort == "WEEKLY_HOTTEST" || searchDto.Sort == "MONTHLY_HOTTEST" ||
                            searchDto.Sort == "HOTTEST",
                    r => (r.ViewHits + r.LikesQuantity * 20 + r.CommentQuantity * 30))
                .OrderByDescending(r => r.CreateTime);

            List<ArticleListDto> articleDtos = articleSelect
                .ToPagerList(searchDto, out long totalCount)
                .Select(a =>
                {
                    ArticleListDto articleDto = _mapper.Map<ArticleListDto>(a);
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

            return new PagedResultDto<ArticleListDto>(articleDtos, totalCount);
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
        public ArticleDto Get(Guid id)
        {
            _articleRepository.UpdateDiy.Set(r => r.ViewHits + 1).Where(r => r.Id == id).ExecuteAffrows();
            return _articleService.Get(id);
        }

        [HttpPost]
        public UnifyResponseDto Post([FromBody] CreateUpdateArticleDto createArticle)
        {
            _articleService.CreateArticle(createArticle);

            _capBus.Publish("NotificationController.Post", new CreateNotificationDto()
            {

            });

            return UnifyResponseDto.Success("新建随笔成功");
        }


        [HttpPut("{id}")]
        public UnifyResponseDto Put(Guid id, [FromBody] CreateUpdateArticleDto updateArticle)
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
            _articleService.UpdateArticle(updateArticle, article);

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
        public UnifyResponseDto Put(Guid id, bool isAudit)
        {
            Article article = _articleRepository.Select.Where(r => r.Id == id).ToOne();
            if (article == null)
            {
                throw new LinCmsException("没有找到相关随笔");
            }

            article.IsAudit = isAudit;
            _articleRepository.Update(article);
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
            long? userId = _currentUser.Id;
            List<long> subscribeUserIds = _userSubscribeRepository.Select.Where(r => r.CreateUserId == userId).ToList(r => r.SubscribeUserId);

            var select = _articleRepository
                .Select
                .Include(r => r.Classify)
                .Include(r => r.UserInfo)
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status))
                .IncludeMany(r => r.UserLikes, r => r.Where(u => u.CreateUserId == userId))
                .Where(r => r.IsAudit)
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status))
                .WhereIf(subscribeUserIds.Count > 0, r => subscribeUserIds.Contains(r.CreateUserId))
                .WhereIf(subscribeUserIds.Count == 0, r => false)
                .OrderByDescending(r => r.CreateTime);


            var articles = select
                .ToPagerList(pageDto, out long totalCount)
                .Select(r =>
                {
                    ArticleListDto articleDto = _mapper.Map<ArticleListDto>(r);
                    articleDto.Tags = r.Tags.Select(r => new TagDto()
                    {
                        TagName = r.TagName,
                        Id = r.Id,
                    }).ToList();

                    articleDto.IsLiked = userId != null && r.UserLikes.Any();

                    articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(articleDto.Thumbnail);
                    return articleDto;
                })
                .ToList();

            return new PagedResultDto<ArticleListDto>(articles, totalCount);
        }
    }
}