using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FreeSql;
using LinCms.Application.Blog.Classifies;
using LinCms.Application.Blog.Tags;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;

namespace LinCms.Application.Blog.Articles
{
    public class ArticleAppService : IArticleService
    {
        private readonly AuditBaseRepository<Article> _articleRepository;
        private readonly GuidRepository<TagArticle> _tagArticleRepository;
        private readonly AuditBaseRepository<UserLike> _userLikeRepository;
        private readonly AuditBaseRepository<Comment> _commentBaseRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IClassifyService _classifyService;
        private readonly ITagService _tagService;

        public ArticleAppService(
                AuditBaseRepository<Article> articleRepository,
                GuidRepository<TagArticle> tagArticleRepository,
                IMapper mapper,
                ICurrentUser currentUser,
                AuditBaseRepository<UserLike> userLikeRepository,
                AuditBaseRepository<Comment> commentBaseRepository,
                IClassifyService classifyService,
                ITagService tagService)
        {
            _articleRepository = articleRepository;
            _tagArticleRepository = tagArticleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userLikeRepository = userLikeRepository;
            _commentBaseRepository = commentBaseRepository;

            _classifyService = classifyService;
            _tagService = tagService;
        }

        public void Delete(Guid id)
        {
            Article article = _articleRepository.Select.Where(r => r.Id == id).IncludeMany(r => r.Tags).ToOne();
            _classifyService.UpdateArticleCount(article.ClassifyId, 1);
            article.Tags.ToList()
                .ForEach(u =>
                {
                    _tagService.UpdateArticleCount(u.Id, -1);
                });

            _articleRepository.Delete(new Article { Id = id });
            _tagArticleRepository.Delete(r => r.ArticleId == id);
            _commentBaseRepository.Delete(r => r.SubjectId == id);
            _userLikeRepository.Delete(r => r.SubjectId == id);
        }

        public ArticleDto Get(Guid id)
        {
            Article article = _articleRepository.Select
                .Include(r => r.Classify)
                .IncludeMany(r => r.Tags).Include(r => r.UserInfo).WhereCascade(r => r.IsDeleted == false).Where(a => a.Id == id).ToOne();

            if (article.IsNull())
            {
                throw new LinCmsException("该随笔不存在");
            }

            ArticleDto articleDto = _mapper.Map<ArticleDto>(article);
            if (articleDto.UserInfo.IsNotNull())
                articleDto.UserInfo.Avatar = _currentUser.GetFileUrl(articleDto.UserInfo.Avatar);

            articleDto.IsLiked = _userLikeRepository.Select.Any(r => r.SubjectId == id && r.CreateUserId == _currentUser.Id);

            articleDto.IsComment = _commentBaseRepository.Select.Any(r => r.SubjectId == id && r.CreateUserId == _currentUser.Id);

            articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(article.Thumbnail);

            return articleDto;
        }

        public void CreateArticle(CreateUpdateArticleDto createArticle)
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
            Article resultArticle = _articleRepository.Insert(article);

            _classifyService.UpdateArticleCount(createArticle.ClassifyId, 1);

        }

        public void UpdateArticle(CreateUpdateArticleDto updateArticleDto, Article article)
        {
            //使用AutoMapper方法简化类与类之间的转换过程
            article.WordNumber = article.Content.Length;
            article.ReadingTime = article.Content.Length / 800;

            _articleRepository.Update(article);

            Article oldArticle = _articleRepository.Select.Where(r => r.Id == article.Id)
                .IncludeMany(r => r.Tags).ToOne();

            oldArticle.Tags.ToList()
                .ForEach(u =>
            {
                _tagService.UpdateArticleCount(u.Id, -1);
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

            _tagArticleRepository.Insert(tagArticles);
            if (article.ClassifyId != updateArticleDto.ClassifyId)
            {
                _classifyService.UpdateArticleCount(article.ClassifyId, -1);
                _classifyService.UpdateArticleCount(updateArticleDto.ClassifyId, 1);
            }
        }
    }
}
