using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Application.Contracts.Blog.Tags.Dtos;
using LinCms.Core.Entities.Blog;
using LinCms.Core.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Repositories.Blog
{
    public class TagRepositoryTest : BaseLinCmsTest
    {

        private readonly IAuditBaseRepository<Tag> _tagRepository;
        private readonly IAuditBaseRepository<TagArticle> _tagArticleRepository;

        public TagRepositoryTest()
        {
            _tagRepository = ServiceProvider.GetService<IAuditBaseRepository<Tag>>();
            _tagArticleRepository = ServiceProvider.GetService<IAuditBaseRepository<TagArticle>>();
        }

        [Fact]
        public void Get()
        {
            //LinUser关联无数据
            var d0 = _tagRepository.Select.Include(r => r.LinUser).ToList<TagListDto>();

            //LinUser关联有数据
            var d1 = _tagRepository.Select.Include(r => r.LinUser).ToList();

            //其他字段都有值，LinUser关联没有数据。
            var d2 = _tagRepository.Select.ToList(r => new TagListDto());
            var d22 = _tagRepository.Select.ToList<TagListDto>(); ;


            //其他字段不取
            var d4 = _tagRepository.Select.ToList(r => new TagListDto() { TagName = r.TagName });

  
            var d3 = _tagRepository.Select.ToList(r => new
            {
                OpenUserDto = new 
                {
                    r.LinUser.Id,
                    r.LinUser.Username,
                    r.LinUser.Nickname
                }
            });


        }


    }
}
