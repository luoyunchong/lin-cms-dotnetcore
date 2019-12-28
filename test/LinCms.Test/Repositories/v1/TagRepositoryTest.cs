using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using LinCms.Test.Controller;
using LinCms.Web.Models.Cms.Users;
using LinCms.Web.Models.v1.Tags;
using LinCms.Zero.Domain;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Repositories
{
    public class TagRepositoryTest : BaseRepositoryTest
    {

        private readonly AuditBaseRepository<Tag> _tagRepository;

        public TagRepositoryTest()
        {
            _tagRepository = serviceProvider.GetService<AuditBaseRepository<Tag>>();
        }

        /// <summary>
        /// 
        /// </summary>
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

            //取出了LinUser，其他字段不取
            var d5 = _tagRepository.Select.ToList(r => new TagDto() { LinUser = r.LinUser });

  
            var d3 = _tagRepository.Select.ToList(r => new
            {
                OpenUserDto = new 
                {
                    r.LinUser.Id,
                    Username = r.LinUser.Username,
                    Nickname = r.LinUser.Nickname
                }
            });


            // 报错
            var d6 = _tagRepository.Select.ToList(r => new TagDto()
            {
                OpenUserDto = new OpenUserDto()
                {
                    Username = r.LinUser.Username,
                    Nickname = r.LinUser.Nickname
                }
            });

        }

    }
}
