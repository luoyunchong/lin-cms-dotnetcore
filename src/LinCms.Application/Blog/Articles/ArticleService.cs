using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
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
    public class ArticleService : IArticleService
    {
        private readonly IAuditBaseRepository<Article> _articleRepository;
        private readonly IAuditBaseRepository<ArticleDraft> _articleDraftRepository;
        private readonly IAuditBaseRepository<UserLike> _userLikeRepository;
        private readonly IAuditBaseRepository<Comment> _commentBaseRepository;
        private readonly IBaseRepository<TagArticle> _tagArticleRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IClassifyService _classifyService;
        private readonly ITagService _tagService;
        private readonly IUserLikeService _userSubscribeService;

        public ArticleService(
            IAuditBaseRepository<Article> articleRepository,
            IBaseRepository<TagArticle> tagArticleRepository,
            IMapper mapper,
            ICurrentUser currentUser,
            IAuditBaseRepository<UserLike> userLikeRepository,
            IAuditBaseRepository<Comment> commentBaseRepository,
            IClassifyService classifyService,
            ITagService tagService, IUserLikeService userSubscribeService,
            IAuditBaseRepository<ArticleDraft> articleDraftRepository)
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
            _articleDraftRepository = articleDraftRepository;
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
                    searchDto.Sort == "THREE_DAYS_HOTTEST" || searchDto.Sort == "WEEKLY_HOTTEST" ||
                    searchDto.Sort == "MONTHLY_HOTTEST" ||
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

        public async Task DeleteAsync(Guid id)
        {
            Article article = _articleRepository.Select.Where(r => r.Id == id).IncludeMany(r => r.Tags).ToOne();
            if (article.IsNotNull())
            {
                await _classifyService.UpdateArticleCountAsync(article.ClassifyId, 1);
                article.Tags?
                    .ForEach(u => { _tagService.UpdateArticleCount(u.Id, -1); });
            }

            await _articleRepository.DeleteAsync(new Article { Id = id });
            await _tagArticleRepository.DeleteAsync(r => r.ArticleId == id);
            await _commentBaseRepository.DeleteAsync(r => r.SubjectId == id);
            await _userLikeRepository.DeleteAsync(r => r.SubjectId == id);
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

            articleDto.IsLiked =
                await _userLikeRepository.Select.AnyAsync(r => r.SubjectId == id && r.CreateUserId == _currentUser.Id);
            articleDto.IsComment =
                await _commentBaseRepository.Select.AnyAsync(
                    r => r.SubjectId == id && r.CreateUserId == _currentUser.Id);
            articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(article.Thumbnail);

            return articleDto;
        }

        public async Task<Guid> CreateAsync(CreateUpdateArticleDto createArticle)
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

            ArticleDraft articleDraft = new ArticleDraft()
            {
                Content = createArticle.Content,
                Editor = createArticle.Editor,
                Title = createArticle.Title
            };

            await _articleRepository.InsertAsync(article);

            articleDraft.Id = article.Id;
            await _articleDraftRepository.InsertAsync(articleDraft);
            await _classifyService.UpdateArticleCountAsync(createArticle.ClassifyId, 1);
            return article.Id;
        }

        public async Task<Article> UpdateAsync(Guid id, CreateUpdateArticleDto updateArticleDto)
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

            _mapper.Map(updateArticleDto, article);
            article.WordNumber = article.Content.Length;
            article.ReadingTime = article.Content.Length / 400;

            await _articleRepository.UpdateAsync(article);
            ArticleDraft articleDraft = _mapper.Map<ArticleDraft>(article);
            await _articleDraftRepository.UpdateAsync(articleDraft);

            return article;
        }


        public async Task UpdateTagAsync(Guid id, CreateUpdateArticleDto updateArticleDto)
        {
            List<Guid> tagIds =
                await _tagArticleRepository.Select.Where(r => r.ArticleId == id).ToListAsync(r => r.TagId);

            tagIds.ForEach(tagId => { _tagService.UpdateArticleCount(tagId, -1); });

            _tagArticleRepository.Delete(r => r.ArticleId == id);

            List<TagArticle> tagArticles = new List<TagArticle>();

            updateArticleDto.TagIds.ForEach(tagId =>
            {
                tagArticles.Add(new TagArticle()
                {
                    ArticleId = id,
                    TagId = tagId
                });
                _tagService.UpdateArticleCount(tagId, 1);
            });
            await _tagArticleRepository.InsertAsync(tagArticles);
        }


        public async Task<PagedResultDto<ArticleListDto>> GetSubscribeArticleAsync(PageDto pageDto)
        {
            long userId = _currentUser.Id ?? 0;
            List<long> subscribeUserIds = await _userSubscribeService.GetSubscribeUserIdAsync(userId);

            var articles = await _articleRepository
                .Select
                .Include(r => r.Classify)
                .Include(r => r.UserInfo)
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status))
                .IncludeMany(r => r.UserLikes, r => r.Where(u => u.CreateUserId == userId))
                .Where(r => r.IsAudit)
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status))
                .WhereIf(subscribeUserIds.Count > 0, r => subscribeUserIds.Contains(r.CreateUserId))
                .WhereIf(subscribeUserIds.Count == 0, r => false)
                .OrderByDescending(r => r.CreateTime).ToPagerListAsync(pageDto, out long totalCount);

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

        public async Task UpdateLikeQuantity(Guid subjectId, int likesQuantity)
        {
            Article article = await _articleRepository.Where(r => r.Id == subjectId).ToOneAsync();
            if (article.IsAudit == false)
            {
                throw new LinCmsException("该文章因违规被拉黑");
            }

            if (likesQuantity < 0)
            {
                //防止数量一直减，减到小于0
                if (article.LikesQuantity < -likesQuantity)
                {
                    return;
                }
            }

            article.LikesQuantity += likesQuantity;
            await _articleRepository.UpdateAsync(article);
            //_articleRepository.UpdateDiy.Set(r => r.LikesQuantity + likesQuantity).Where(r => r.Id == subjectId).ExecuteAffrows();
        }
    }
}