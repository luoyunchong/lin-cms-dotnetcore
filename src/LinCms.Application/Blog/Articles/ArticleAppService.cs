using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using LinCms.Application.Blog.Classifies;
using LinCms.Application.Blog.Tags;
using LinCms.Application.Blog.UserSubscribes;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Application.Contracts.Blog.Articles.Dtos;
using LinCms.Application.Contracts.Blog.Classifys;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Application.Blog.Articles
{
    public class ArticleAppService : IArticleService
    {
        private readonly IAuditBaseRepository<Article> _articleRepository;
        private readonly IAuditBaseRepository<UserLike> _userLikeRepository;
        private readonly IAuditBaseRepository<Comment> _commentBaseRepository;
        private readonly IBaseRepository<TagArticle> _tagArticleRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IClassifyService _classifyService;
        private readonly ITagService _tagService;
        private readonly IUserSubscribeService _userSubscribeService;

        public ArticleAppService(
                IAuditBaseRepository<Article> articleRepository,
                IBaseRepository<TagArticle> tagArticleRepository,
                IMapper mapper,
                ICurrentUser currentUser,
                IAuditBaseRepository<UserLike> userLikeRepository,
                IAuditBaseRepository<Comment> commentBaseRepository,
                IClassifyService classifyService,
                ITagService tagService, IUserSubscribeService userSubscribeService)
        {
            _articleRepository = articleRepository;
            _tagArticleRepository = tagArticleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userLikeRepository = userLikeRepository;
            _commentBaseRepository = commentBaseRepository;

            _classifyService = classifyService;
            _tagService = tagService;
            _userSubscribeService = userSubscribeService;
        }

        public async Task<PagedResultDto<ArticleListDto>> GetArticleAsync(ArticleSearchDto searchDto)
        {
            DateTime monthDays = DateTime.Now.AddDays(-30);
            DateTime weeklyDays = DateTime.Now.AddDays(-7);
            DateTime threeDays = DateTime.Now.AddDays(-3);

            long? userId = _currentUser.Id;
            List<Article> articles = await _articleRepository
                .Select
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
                .OrderByDescending(r => r.CreateTime).ToPagerListAsync(searchDto, out long totalCount);

            List<ArticleListDto> articleDtos = articles
                .Select(a =>
                {
                    ArticleListDto articleDto = _mapper.Map<ArticleListDto>(a);

                    articleDto.IsLiked = userId != null && a.UserLikes.Any();
                    articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(articleDto.Thumbnail);

                    return articleDto;
                })
                .ToList();

            return new PagedResultDto<ArticleListDto>(articleDtos, totalCount);
        }

        public void Delete(Guid id)
        {
            Article article = _articleRepository.Select.Where(r => r.Id == id).IncludeMany(r => r.Tags).ToOne();
            if (article.IsNotNull())
            {
                _classifyService.UpdateArticleCount(article.ClassifyId, 1);
                article.Tags?
                    .ForEach(u =>
                    {
                        _tagService.UpdateArticleCount(u.Id, -1);
                    });
            }

            _articleRepository.Delete(new Article { Id = id });
            _tagArticleRepository.Delete(r => r.ArticleId == id);
            _commentBaseRepository.Delete(r => r.SubjectId == id);
            _userLikeRepository.Delete(r => r.SubjectId == id);
        }

        public async Task<ArticleDto> GetAsync(Guid id)
        {
            Article article = await _articleRepository.Select
                .Include(r => r.Classify)
                .IncludeMany(r => r.Tags)
                .Include(r => r.UserInfo).WhereCascade(r => r.IsDeleted == false).Where(a => a.Id == id).ToOneAsync();

            if (article.IsNull())
            {
                throw new LinCmsException("该随笔不存在");
            }

            ArticleDto articleDto = _mapper.Map<ArticleDto>(article);

            if (articleDto.Tags.IsNotNull())
            {
                articleDto.Tags.ForEach(r => { r.ThumbnailDisplay = _currentUser.GetFileUrl(r.Thumbnail); });
            }

            if (articleDto.UserInfo.IsNotNull())
            {
                articleDto.UserInfo.Avatar = _currentUser.GetFileUrl(articleDto.UserInfo.Avatar);
            }

            articleDto.IsLiked = await _userLikeRepository.Select.AnyAsync(r => r.SubjectId == id && r.CreateUserId == _currentUser.Id);
            articleDto.IsComment = await _commentBaseRepository.Select.AnyAsync(r => r.SubjectId == id && r.CreateUserId == _currentUser.Id);
            articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(article.Thumbnail);

            return articleDto;
        }

        public async Task CreateAsync(CreateUpdateArticleDto createArticle)
        {
            Article article = _mapper.Map<Article>(createArticle);
            article.Archive = DateTime.Now.ToString("yyy年MM月");
            article.WordNumber = createArticle.Content.Length;
            article.ReadingTime = createArticle.Content.Length / 800;

            article.Tags = new List<Tag>();
            foreach (var articleTagId in createArticle.TagIds)
            {
                article.Tags.Add(new Tag()
                {
                    Id = articleTagId,
                });
                _tagService.UpdateArticleCount(articleTagId, 1);
            }

            await _articleRepository.InsertAsync(article);

            _classifyService.UpdateArticleCount(createArticle.ClassifyId, 1);

        }

        public async Task UpdateAsync(CreateUpdateArticleDto updateArticleDto, Article article)
        {
            article.WordNumber = article.Content.Length;
            article.ReadingTime = article.Content.Length / 800;

            await _articleRepository.UpdateAsync(article);

            List<Guid> tagIds = await _tagArticleRepository.Select
                                .Where(r => r.ArticleId == article.Id)
                                .ToListAsync(r => r.TagId);

            tagIds.ForEach(tagId =>
            {
                _tagService.UpdateArticleCount(tagId, -1);
            });
            _tagArticleRepository.Delete(r => r.ArticleId == article.Id);

            List<TagArticle> tagArticles = new List<TagArticle>();
            updateArticleDto.TagIds.ForEach(tagId =>
                {
                    tagArticles.Add(new TagArticle()
                    {
                        ArticleId = article.Id,
                        TagId = tagId
                    });
                    _tagService.UpdateArticleCount(tagId, 1);
                });
            await _tagArticleRepository.InsertAsync(tagArticles);

            if (article.ClassifyId != updateArticleDto.ClassifyId)
            {
                _classifyService.UpdateArticleCount(article.ClassifyId, -1);
                _classifyService.UpdateArticleCount(updateArticleDto.ClassifyId, 1);
            }
        }


        public PagedResultDto<ArticleListDto> GetSubscribeArticle(PageDto pageDto)
        {
            long userId = _currentUser.Id ?? 0;
            List<long> subscribeUserIds = _userSubscribeService.GetSubscribeUserId(userId);

            var articles = _articleRepository
                .Select
                .Include(r => r.Classify)
                .Include(r => r.UserInfo)
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status))
                .IncludeMany(r => r.UserLikes, r => r.Where(u => u.CreateUserId == userId))
                .Where(r => r.IsAudit)
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status))
                .WhereIf(subscribeUserIds.Count > 0, r => subscribeUserIds.Contains(r.CreateUserId))
                .WhereIf(subscribeUserIds.Count == 0, r => false)
                .OrderByDescending(r => r.CreateTime).ToPagerList(pageDto, out long totalCount);

            List<ArticleListDto> articleDtos = articles
                .Select(r =>
                {
                    ArticleListDto articleDto = _mapper.Map<ArticleListDto>(r);
                    articleDto.IsLiked = r.UserLikes.Any();
                    articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(articleDto.Thumbnail);
                    return articleDto;
                })
                .ToList();

            return new PagedResultDto<ArticleListDto>(articleDtos, totalCount);
        }
    }
}
