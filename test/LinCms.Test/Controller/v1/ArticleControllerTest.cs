using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Web.Controllers.v1;
using LinCms.Web.Models.v1.Articles;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Controller.v1
{
    public class ArticleControllerTest : BaseControllerTests
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly AuditBaseRepository<Article> _articleRepository;
        private readonly AuditBaseRepository<Tag> _tagRepository;
        private readonly ArticleController _articleController;

        public ArticleControllerTest() : base()
        {
            _hostingEnv = serviceProvider.GetService<IWebHostEnvironment>();
            _articleController = serviceProvider.GetService<ArticleController>(); ;

            _mapper = serviceProvider.GetService<IMapper>();
            _articleRepository = serviceProvider.GetService<AuditBaseRepository<Article>>();
            _tagRepository = serviceProvider.GetService<AuditBaseRepository<Tag>>();
            _freeSql = serviceProvider.GetService<IFreeSql>();
        }



        /// <summary>
        /// 使用BaseItem某一类别作为文件分类专栏时，使用From来join出分类专栏的名称。
        /// </summary>
        [Fact]
        public void Gets()
        {
            //不使用关联属性获取文章专栏
            List<ArticleDto> articleDtos = _articleRepository
                .Select
                .From<Classify>((a, b) =>
                    a.LeftJoin(r => r.ClassifyId == b.Id)
                ).ToList((s, a) => new
                {
                    Article = s,
                    a.ClassifyName
                })
                .Select(r =>
                {
                    ArticleDto articleDto = _mapper.Map<ArticleDto>(r.Article);
                    return articleDto;
                }).ToList();

            //使用SQL直接获取文章及其分类名称
            List<ArticleDto> t9 = _freeSql.Ado.Query<ArticleDto>($@"
                            SELECT a.*,b.item_name as classifyName 
                            FROM blog_article a 
                            LEFT JOIN base_item b 
                            on a.classify_id=b.id where a.is_deleted=0"
            );

            //属性Classify为null
            List<Article> articles1 = _articleRepository
                .Select
                .ToList();

            //属性Classify会有值
            List<Article> articles2 = _articleRepository
                .Select
                .Include(r => r.Classify)
                .ToList();

            //配合IMapper，转换为ArticleDto
            List<ArticleDto> articles3 = _articleRepository
                .Select
                .ToList(r => new
                {
                    r.Classify,
                    Article = r
                }).Select(r =>
                {
                    ArticleDto articleDto = _mapper.Map<ArticleDto>(r.Article);
                    return articleDto;
                }).ToList();


            List<ArticleDto> articles4 = _articleRepository
                .Select
                .Include(r => r.Classify)
                .ToList().Select(r =>
                {
                    ArticleDto articleDto = _mapper.Map<ArticleDto>(r);
                    return articleDto;
                }).ToList();


        }

        [Fact]
        public void TestInCludeMany()
        {
            var d0 = _articleRepository
                .Select
                .IncludeMany(r => r.Tags).ToList();

            var d1 = _articleRepository
                .Select.Include(r => r.Classify)
                .ToList();


            var d2 = _articleRepository
                .Select.Include(r => r.Classify).IncludeMany(r => r.Tags)
                .ToList(true);


            var d3 = _articleRepository
                .Select
                .ToList(r => new
                {
                    r.Classify,
                    r
                });

            var d5 = _articleRepository
                .Select
                .Include(r => r.Classify)
                .IncludeMany(r => r.Tags)
                .ToList(r => new
                {
                    r,
                });

            var d4 = _articleRepository
                .Select
                .ToList();

        }

        [Fact]
        public void Test()
        {
            Guid guid = new Guid("5dc93286-5e44-c190-008e-3fc74d4fcee0");
            var error = _articleRepository.Select.IncludeMany(r => r.Tags)
                .Where(r => r.Id == guid)
                .ToList(); ;
        }

        [Fact]
        public void Test2()
        {
            //blog_article 找不到列 Tags”
            Guid guid = new Guid("5dc93286-5e44-c190-008e-3fc74d4fcee0");
            var error = _articleRepository.Select
                .Where(r => r.Id == guid)
                .ToList(r => r.Tags);
        }


        [Fact]
        public void TestWhereOneToMany()
        {
            ArticleSearchDto searchDto = new ArticleSearchDto()
            {
                TagId = new Guid("5dc931fd-5e44-c190-008e-3fc4728735d6")
            };

            var data1 = _articleRepository
                .Select
                .IncludeMany(r => r.Tags)
                .Where(r => r.Tags.AsSelect().Any(u => u.Id == searchDto.TagId)).ToList();

            var data2 = _articleRepository
                .Select
                .Where(r => r.Tags.AsSelect().Where(u => u.Id == searchDto.TagId).Count() > 0).ToList();

            var data3 = _articleRepository
                .Select
                .IncludeMany(r => r.Tags.Take(10)).ToList();

            var data4 = _articleRepository
                .Select
                .IncludeMany(r => r.Tags).ToList();

            //注意这里where填写的是u=>u.Id==r.Id
            var data5 = _articleRepository
                .Select
                .IncludeMany(r => r.Tags.Where(u => u.Id == r.Id)).ToList();

            //System.Exception:“表达式错误，它不是连续的 MemberAccess 类型：value(LinCms.Test.Controller.v1.ArticleControllerTest+<>c__DisplayClass9_0).searchDto”
            var data = _articleRepository
                .Select
                .IncludeMany(r => r.Tags.Where(u => u.Id == searchDto.TagId)).ToList();

        }

        [Fact]
        public void Post()
        {
            CreateUpdateArticleDto createArticle=new CreateUpdateArticleDto();
            _articleController.Post(createArticle);
        }
    }
}
