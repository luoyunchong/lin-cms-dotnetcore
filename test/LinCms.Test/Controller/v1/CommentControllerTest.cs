using System;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.v1.Comments;
using LinCms.Zero.Domain.Blog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Controller.v1
{
    public class CommentControllerTest : BaseControllerTests
    {
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly BaseRepository<Comment> _baseRepository;

        public CommentControllerTest() : base()
        {
            _hostingEnv = serviceProvider.GetService<IHostingEnvironment>();

            _mapper = serviceProvider.GetService<IMapper>();
            _baseRepository = serviceProvider.GetService<BaseRepository<Comment>>();
            _freeSql = serviceProvider.GetService<IFreeSql>();
        }


        [Fact]
        public void Gets()
        {
            CommentSearchDto commentSearchDto = new CommentSearchDto()
            {
                ArticleId = Guid.Parse("5dc6bb5f-9cfa-ff24-00a8-d50e576b275a")
            };
            dynamic comments = _baseRepository
                .Select
                .Include(r => r.UserInfo)
                .From<UserLike>(
                    (z, zzc) =>
                        z.LeftJoin(u => u.Id == zzc.SubjectId)
                )
                .ToList((s, za) => new
                {
                    comment = s,
                    za.SubjectId
                });
        }

        [Fact]
        public void GetComments()
        {
            dynamic comments = _freeSql.Select<Comment>().Include(r => r.UserInfo).From<UserLike>(
                         (z, zzc) =>
                             z.LeftJoin(u => u.Id == zzc.SubjectId)//&& zzc.CreateUserId == _currentUser.Id
                     )
                     .ToList((s, za) => new
                     {
                         comment = s,
                         za.SubjectId
                     });
        }
    }
}
