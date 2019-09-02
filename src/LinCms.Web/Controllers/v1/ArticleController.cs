using System.Linq;
using AutoMapper;
using LinCms.Web.Models.v1.Articles;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
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
        private readonly IMapper _mapper;
        public ArticleController(AuditBaseRepository<Article> articleRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除随笔", "随笔")]
        public ResultDto DeleteArticle(int id)
        {
            _articleRepository.Delete(new Article { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        public PagedResultDto<ArticleDto> Get([FromQuery]PageDto pageDto)
        {
            var articles= _articleRepository.Select.OrderByDescending(r => r.Id).ToPagerList(pageDto, out var totalCount).ToList()
            .Select(r => _mapper.Map<ArticleDto>(r)).ToList();   

            return new PagedResultDto<ArticleDto>(articles, totalCount);
        }

        [HttpGet("{id}")]
        public ArticleDto Get(int id)
        {
            Article article = _articleRepository.Select.Where(a => a.Id == id).ToOne();
            return _mapper.Map<ArticleDto>(article);
        }

        [LinCmsAuthorize("新增随笔", "随笔")]
        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateArticleDto createArticle)
        {
            bool exist = _articleRepository.Select.Any(r => r.Title == createArticle.Title);
            if (exist)
            {
                throw new LinCmsException("随笔标题不能重复");
            }

            Article article = _mapper.Map<Article>(createArticle);
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

            bool exist = _articleRepository.Select.Any(r => r.Title == updateArticle.Title && r.Id != id);
            if (exist)
            {
                throw new LinCmsException("随笔已存在");
            }

            //使用AutoMapper方法简化类与类之间的转换过程
            _mapper.Map(updateArticle, article);

            _articleRepository.Update(article);

            return ResultDto.Success("更新随笔成功");
        }
    }
}