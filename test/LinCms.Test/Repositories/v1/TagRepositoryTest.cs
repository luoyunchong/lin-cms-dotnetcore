using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Core.Entities.Blog;
using LinCms.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Repositories.v1
{
    public class TagRepositoryTest : BaseLinCmsTest
    {

        private readonly AuditBaseRepository<Tag> _tagRepository;

        public TagRepositoryTest()
        {
            _tagRepository = ServiceProvider.GetService<AuditBaseRepository<Tag>>();
        }

        [Fact]
        public void Get()
        {
            //LinUser关联无数据
            var d0 = _tagRepository.Select.Include(r => r.LinUser).ToList<TagDto>();

            //LinUser关联有数据
            var d1 = _tagRepository.Select.Include(r => r.LinUser).ToList();

            //其他字段都有值，LinUser关联没有数据。
            var d2 = _tagRepository.Select.ToList(r => new TagDto());
            var d22 = _tagRepository.Select.ToList<TagDto>(); ;


            //其他字段不取
            var d4 = _tagRepository.Select.ToList(r => new TagDto() { TagName = r.TagName });

  
            var d3 = _tagRepository.Select.ToList(r => new
            {
                OpenUserDto = new 
                {
                    r.LinUser.Id,
                    Username = r.LinUser.Username,
                    Nickname = r.LinUser.Nickname
                }
            });


        }

    }
}
