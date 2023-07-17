using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Blog.Classifys;
using LinCms.Blog.UserSubscribes;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Blog.Articles;

public class ArticleService : ApplicationService, IArticleService
{
    #region Constructor
    private readonly IAuditBaseRepository<Article> _articleRepository;
    private readonly IAuditBaseRepository<ArticleDraft> _articleDraftRepository;
    private readonly IAuditBaseRepository<UserLike> _userLikeRepository;
    private readonly IAuditBaseRepository<ArticleCollection> _articleCollectionRepository;
    private readonly IAuditBaseRepository<Comment> _commentRepository;
    private readonly IAuditBaseRepository<TagArticle> _tagArticleRepository;
    private readonly IClassifyService _classifyService;
    private readonly IAuditBaseRepository<Tag> _tagRepository;
    private readonly IUserSubscribeService _userSubscribeService;
    private readonly IFileRepository _fileRepository;

    public ArticleService(
        IAuditBaseRepository<Article> articleRepository,
        IAuditBaseRepository<TagArticle> tagArticleRepository,
        IAuditBaseRepository<UserLike> userLikeRepository,
        IAuditBaseRepository<Comment> commentRepository,
        IClassifyService classifyService,
        IAuditBaseRepository<Tag> tagRepository,
        IUserSubscribeService userSubscribeService,
        IAuditBaseRepository<ArticleDraft> articleDraftRepository,
        IFileRepository fileRepository, IAuditBaseRepository<ArticleCollection> articleCollectionRepository)
    {
        _articleRepository = articleRepository;
        _tagArticleRepository = tagArticleRepository;
        _userLikeRepository = userLikeRepository;
        _commentRepository = commentRepository;

        _classifyService = classifyService;
        _tagRepository = tagRepository;
        _userSubscribeService = userSubscribeService;
        _articleDraftRepository = articleDraftRepository;
        _fileRepository = fileRepository;
        _articleCollectionRepository = articleCollectionRepository;
    }
    #endregion

    #region CRUD
    public async Task<PagedResultDto<ArticleListDto>> GetArticleAsync(ArticleSearchDto searchDto)
    {
        DateTime monthDays = DateTime.Now.AddDays(-30);
        DateTime weeklyDays = DateTime.Now.AddDays(-7);
        DateTime threeDays = DateTime.Now.AddDays(-3);

        var articleIds = new List<Guid>();

        if (searchDto.CollectionId != null)
        {
            articleIds = _articleCollectionRepository.Select.Where(r => r.CollectionId == searchDto.CollectionId).ToList(r => r.ArticleId);
        }

        long? userId = CurrentUser.FindUserId();
        List<Article> articles = await _articleRepository
            .Select
            .Include(r => r.UserInfo)
            .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
            .IncludeMany(r => r.UserLikes, r => r.Where(u => u.CreateUserId == userId))
            .Where(r => r.IsAudit)
            .WhereCascade(r => r.IsDeleted == false)
            .WhereIf(searchDto.UserId != null, r => r.CreateUserId == searchDto.UserId)
            .WhereIf(searchDto.TagId.HasValue, r => r.Tags.AsSelect().Any(u => u.Id == searchDto.TagId))
            .WhereIf(searchDto.ClassifyId.HasValue, r => r.ClassifyId == searchDto.ClassifyId)
            .WhereIf(searchDto.ChannelId.HasValue, r => r.ChannelId == searchDto.ChannelId)
            .WhereIf(searchDto.Title.IsNotNullOrEmpty(), r => r.Title.Contains(searchDto.Title))
            .WhereIf(searchDto.Sort == "THREE_DAYS_HOTTEST", r => r.CreateTime > threeDays)
            .WhereIf(searchDto.Sort == "WEEKLY_HOTTEST", r => r.CreateTime > weeklyDays)
            .WhereIf(searchDto.Sort == "MONTHLY_HOTTEST", r => r.CreateTime > monthDays)
            .WhereIf(searchDto.CollectionId != null, r => articleIds.Contains(r.Id))
            .OrderByDescending(
                searchDto.Sort == "THREE_DAYS_HOTTEST" || searchDto.Sort == "WEEKLY_HOTTEST" ||
                searchDto.Sort == "MONTHLY_HOTTEST" ||
                searchDto.Sort == "HOTTEST",
                r => r.ViewHits + (r.LikesQuantity * 20) + (r.CommentQuantity * 30))
            .OrderByDescending(r => r.CreateTime).ToPagerListAsync(searchDto, out long totalCount);

        List<ArticleListDto> articleDtos = articles
            .Select(a =>
            {
                ArticleListDto articleDto = Mapper.Map<ArticleListDto>(a);

                articleDto.IsLiked = userId != null && a.UserLikes.Any();
                articleDto.ThumbnailDisplay = _fileRepository.GetFileUrl(articleDto.Thumbnail);

                return articleDto;
            })
            .ToList();

        return new PagedResultDto<ArticleListDto>(articleDtos, totalCount);
    }

    [Transactional]
    public async Task DeleteAsync(Guid id)
    {
        Article article = await _articleRepository.Select.Where(r => r.Id == id).IncludeMany(r => r.Tags).FirstAsync();
        if (article.IsNotNull())
        {
            await _classifyService.UpdateArticleCountAsync(article.ClassifyId, 1);
            foreach (var u in article.Tags)
            {
                await UpdateArticleCountAsync(u.Id, -1);
            }
        }

        await _articleRepository.DeleteAsync(new Article { Id = id });
        await _tagArticleRepository.DeleteAsync(r => r.ArticleId == id);
        await _commentRepository.DeleteAsync(r => r.SubjectId == id);
        await _userLikeRepository.DeleteAsync(r => r.SubjectId == id);
    }

    public async Task<ArticleDto> GetAsync(Guid id)
    {
        Article article = await _articleRepository.Select
            .Include(r => r.Classify).IncludeMany(r => r.Tags).Include(r => r.UserInfo)
            .WhereCascade(r => r.IsDeleted == false).Where(a => a.Id == id).ToOneAsync();

        if (article.IsNull())
        {
            throw new LinCmsException("该随笔不存在");
        }

        ArticleDto articleDto = Mapper.Map<ArticleDto>(article);

        if (articleDto.Tags.IsNotNull())
        {
            articleDto.Tags.ForEach(r => { r.ThumbnailDisplay = _fileRepository.GetFileUrl(r.Thumbnail); });
        }

        if (articleDto.UserInfo.IsNotNull())
        {
            articleDto.UserInfo.Avatar = _fileRepository.GetFileUrl(articleDto.UserInfo.Avatar);
        }

        articleDto.IsLiked =
            await _userLikeRepository.Select.AnyAsync(r =>
                r.SubjectId == id && r.CreateUserId == CurrentUser.FindUserId());
        articleDto.IsComment =
            await _commentRepository.Select.AnyAsync(r =>
                r.SubjectId == id && r.CreateUserId == CurrentUser.FindUserId());
        articleDto.IsCollect =
            await _articleCollectionRepository.Select.AnyAsync(r =>
                r.ArticleId == id && r.CreateUserId == CurrentUser.FindUserId());
        articleDto.ThumbnailDisplay = _fileRepository.GetFileUrl(article.Thumbnail);

        return articleDto;
    }

    [Transactional]
    public async Task<Guid> CreateAsync(CreateUpdateArticleDto createArticle)
    {
        Article article = Mapper.Map<Article>(createArticle);
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
            await UpdateArticleCountAsync(articleTagId, 1);
        }

        await _articleRepository.InsertAsync(article);

        await _articleDraftRepository.InsertAsync(new ArticleDraft(article.Id, createArticle.Content,
            createArticle.Title, createArticle.Editor));

        if (createArticle.ClassifyId != null)
        {
            await _classifyService.UpdateArticleCountAsync(createArticle.ClassifyId, 1);
        }

        return article.Id;
    }

    [Transactional]
    public async Task UpdateAsync(Guid id, CreateUpdateArticleDto updateArticleDto)
    {
        Article article = _articleRepository.Select.Where(r => r.Id == id).ToOne();


        if (article.CreateUserId != CurrentUser.FindUserId())
        {
            throw new LinCmsException("您无权修改他人的随笔");
        }

        if (article == null)
        {
            throw new LinCmsException("没有找到相关随笔");
        }

        if (article.ClassifyId != updateArticleDto.ClassifyId)
        {
            await _classifyService.UpdateArticleCountAsync(article.ClassifyId, -1);
            await _classifyService.UpdateArticleCountAsync(updateArticleDto.ClassifyId, 1);
        }

        Mapper.Map(updateArticleDto, article);
        article.WordNumber = article.Content.Length;
        article.ReadingTime = article.Content.Length / 800;
        await _articleRepository.UpdateAsync(article);

        ArticleDraft articleDraft = Mapper.Map<ArticleDraft>(article);
        bool exist = await _articleDraftRepository.Select.AnyAsync(r => r.Id == article.Id);
        if (exist)
        {
            await _articleDraftRepository.UpdateAsync(articleDraft);
        }
        else
        {
            await _articleDraftRepository.InsertAsync(articleDraft);
        }

        await UpdateTagAsync(id, updateArticleDto);
    }

    #endregion

    /// <summary>
    ///  随笔选择多个标签
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateArticleDto"></param>
    /// <returns></returns>
    private async Task UpdateTagAsync(Guid id, CreateUpdateArticleDto updateArticleDto)
    {
        List<Guid> tagIds = await _tagArticleRepository.Select.Where(r => r.ArticleId == id).ToListAsync(r => r.TagId);

        foreach (var tagId in tagIds)
        {
            await UpdateArticleCountAsync(tagId, -1);
        }

        _tagArticleRepository.Delete(r => r.ArticleId == id);

        List<TagArticle> tagArticles = new();

        foreach (var tagId in updateArticleDto.TagIds)
        {
            tagArticles.Add(new TagArticle()
            {
                ArticleId = id,
                TagId = tagId
            });
            await UpdateArticleCountAsync(tagId, 1);
        }

        await _tagArticleRepository.InsertAsync(tagArticles);
    }

    private async Task UpdateArticleCountAsync(Guid? id, int inCreaseCount)
    {
        if (id == null)
        {
            return;
        }

        Tag tag = await _tagRepository.Select.Where(r => r.Id == id).ToOneAsync();
        if (tag == null)
        {
            return;
        }

        //防止数量一直减，减到小于0
        if (inCreaseCount < 0)
        {
            if (tag.ArticleCount < -inCreaseCount)
            {
                return;
            }
        }

        tag.ArticleCount = tag.ArticleCount + inCreaseCount;

        await _tagRepository.UpdateAsync(tag);
    }

    public async Task<PagedResultDto<ArticleListDto>> GetSubscribeArticleAsync(PageDto pageDto)
    {
        long userId = CurrentUser.FindUserId() ?? 0;
        List<long> subscribeUserIds = await _userSubscribeService.GetSubscribeUserIdAsync(userId);

        var articles = await _articleRepository
            .Select
            .Include(r => r.Classify)
            .Include(r => r.UserInfo)
            .IncludeMany(r => r.Tags, r => r.Where(u => u.Status))
            .IncludeMany(r => r.UserLikes) //, r => r.Where(u => u.CreateUserId == userId))
            .Where(r => r.IsAudit)
            .WhereIf(subscribeUserIds.Count > 0, r => subscribeUserIds.Contains(r.CreateUserId.Value))
            .WhereIf(subscribeUserIds.Count == 0, r => false)
            .OrderByDescending(r => r.CreateTime).ToPagerListAsync(pageDto, out long totalCount);

        List<ArticleListDto> articleDtos = articles
            .Select(r =>
            {
                ArticleListDto articleDto = Mapper.Map<ArticleListDto>(r);
                articleDto.IsLiked = r.UserLikes.Any();
                articleDto.ThumbnailDisplay = _fileRepository.GetFileUrl(articleDto.Thumbnail);
                return articleDto;
            })
            .ToList();

        return new PagedResultDto<ArticleListDto>(articleDtos, totalCount);
    }

    public async Task UpdateLikeQuantityAysnc(Guid articleId, int likesQuantity)
    {
        Article article = await _articleRepository.Where(r => r.Id == articleId).ToOneAsync();
        if (article == null) return;
        article.UpdateLikeQuantity(likesQuantity);
        await _articleRepository.UpdateAsync(article);
    }

    public async Task UpdateCollectQuantityAysnc(Guid articleId, int collectQuantity)
    {
        Article article = await _articleRepository.Where(r => r.Id == articleId).ToOneAsync();
        if (article == null) return;
        article.UpdateCollectQuantity(collectQuantity);
        await _articleRepository.UpdateAsync(article);
    }

    public async Task UpdateCommentable(Guid id, bool commentable)
    {
        Article article = await _articleRepository.Select.Where(a => a.Id == id).ToOneAsync();
        if (article == null)
        {
            throw new LinCmsException("没有找到相关随笔", ErrorCode.NotFound);
        }

        if (article.CreateUserId != CurrentUser.FindUserId())
        {
            throw new LinCmsException("不是自己的随笔", ErrorCode.NoPermission);
        }

        article.Commentable = commentable;
        await _articleRepository.UpdateAsync(article);
    }
}