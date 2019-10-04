using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.v1.Articles;
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
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IFreeSql _freeSql;
        public ArticleController(AuditBaseRepository<Article> articleRepository, IMapper mapper, ICurrentUser currentUser, GuidRepository<TagArticle> tagArticleRepository, IFreeSql freeSql)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _tagArticleRepository = tagArticleRepository;
            _freeSql = freeSql;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除随笔", "随笔")]
        public ResultDto DeleteArticle(int id)
        {
            _tagArticleRepository.Delete(r => r.ArticleId == id);
            _articleRepository.Delete(new Article { Id = id });
            return ResultDto.Success();
        }

        /// <summary>
        /// 随笔列表页
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        [HttpGet]
        public PagedResultDto<ArticleDto> Get([FromQuery]PageDto pageDto)
        {
            List<ArticleDto> articles = _articleRepository
                .Select
                .Where(r=>r.CreateUserId==_currentUser.Id)
                .OrderByDescending(r=>r.IsStickie)
                .OrderByDescending(r => r.Id)
                .ToPagerList(pageDto, out long totalCount)
                .ToList()
                .Select(r => _mapper.Map<ArticleDto>(r)).ToList();

            return new PagedResultDto<ArticleDto>(articles, totalCount);
        }

        /// <summary>
        /// 随笔详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ArticleDto Get(int id)
        {
            Article article = _articleRepository.Select.Where(a => a.Id == id).ToOne();

            ArticleDto articleDto = _mapper.Map<ArticleDto>(article);

            articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(article.Thumbnail);

            articleDto.TagIds = _tagArticleRepository.Select.Where(r => r.ArticleId == id).ToList(r => r.TagId);

            return articleDto;
        }

        [LinCmsAuthorize("新增随笔", "随笔")]
        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateArticleDto createArticle)
        {
            bool exist = _articleRepository.Select.Any(r => r.Title == createArticle.Title && r.CreateUserId == _currentUser.Id);
            if (exist)
            {
                throw new LinCmsException("随笔标题不能重复");
            }

            Article article = _mapper.Map<Article>(createArticle);
            article.Archive = DateTime.Now.ToString("yyy年MM月");
            if (article.Author.IsNullOrEmpty())
            {
                article.Author = _currentUser.UserName;
            }

            article.TagArticles = new List<TagArticle>();
            foreach (var articleTagId in createArticle.TagIds)
            {
                article.TagArticles.Add(new TagArticle()
                {
                    TagId = articleTagId,
                });
            }
            _articleRepository.Insert(article);

            return ResultDto.Success("新建随笔成功");
        }

        [LinCmsAuthorize("修改随笔", "随笔")]
        [HttpPut("{id}")]
        public ResultDto Put(int id, [FromBody] CreateUpdateArticleDto updateArticle)
        {
            Article article = _articleRepository.Select.Where(r => r.Id == id).ToOne();
            if (article == null)
            {
                throw new LinCmsException("没有找到相关随笔");
            }

            bool exist = _articleRepository.Select.Any(r => r.Title == updateArticle.Title && r.Id != id && r.CreateUserId == _currentUser.Id);
            if (exist)
            {
                throw new LinCmsException("随笔已存在");
            }

            //使用AutoMapper方法简化类与类之间的转换过程
            _mapper.Map(updateArticle, article);

   
            _articleRepository.Update(article);

            _tagArticleRepository.Delete(r => r.ArticleId == id);

            List<TagArticle> tagArticles = new List<TagArticle>();

            updateArticle.TagIds.ForEach(r => tagArticles.Add(new TagArticle()
            {
                ArticleId = id,
                TagId = r
            }));

            _tagArticleRepository.Insert(tagArticles);

            return ResultDto.Success("更新随笔成功");
            //throw new LinCmsException("test");
        }
    }
}