using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System;
using AutoMapper;
using FreeSql;
using LinCms.Application.Contracts.Blog.Comments;
using LinCms.Application.Contracts.Blog.Comments.Dtos;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Extensions;

namespace LinCms.Test.Repositories.Blog
{
    public class CommentRepositoryTest : BaseLinCmsTest
    {
        private readonly BaseRepository<Comment> _baseRepository;
        
        private readonly IMapper Mapper;
        private readonly IFreeSql FreeSql;

        public CommentRepositoryTest() : base()
        {
            _baseRepository = ServiceProvider.GetRequiredService<BaseRepository<Comment>>();
            Mapper = ServiceProvider.GetRequiredService<IMapper>();
            FreeSql = ServiceProvider.GetRequiredService<IFreeSql>();
        }


        [Fact]
        public void Gets()
        {
            CommentSearchDto commentSearchDto = new CommentSearchDto()
            {
                SubjectId = Guid.Parse("5dc6bb5f-9cfa-ff24-00a8-d50e576b275a")
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
            dynamic comments = FreeSql.Select<Comment>().Include(r => r.UserInfo).From<UserLike>(
                         (z, zzc) =>
                             z.LeftJoin(u => u.Id == zzc.SubjectId)//&& zzc.CreateUserId == _currentUser.Id
                     )
                     .ToList((s, za) => new
                     {
                         comment = s,
                         za.SubjectId
                     });
        }

        [Fact]
        public void Test()
        {
            CommentSearchDto commentSearchDto = new CommentSearchDto()
            {
                SubjectId = Guid.Parse("5dc6bb5f-9cfa-ff24-00a8-d50e576b275a")
            };

            long userId = 7;
            List<CommentDto> comments = _baseRepository
                .Select
                .Include(r => r.UserInfo)
                .Include(r => r.RespUserInfo)
                .IncludeMany(r => r.Childs.Take(2), t => t.Include(u => u.UserInfo))
                .IncludeMany(r => r.UserLikes)
                .WhereIf(commentSearchDto.SubjectId != null, r => r.SubjectId == commentSearchDto.SubjectId)
                .Where(r => r.RootCommentId == commentSearchDto.RootCommentId)
                .OrderByDescending(!commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                .OrderBy(commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                .ToPagerList(commentSearchDto, out long totalCount)
                .Select(r =>
                {
                    CommentDto commentDto = Mapper.Map<CommentDto>(r);


                    commentDto.TopComment = r.Childs.ToList().Select(u =>
                    {
                        CommentDto childrenDto = Mapper.Map<CommentDto>(u);
                        return childrenDto;
                    }).ToList();
                    commentDto.IsLiked = r.UserLikes.Where(u => u.CreateUserId == userId).IsNotEmpty();
                    return commentDto;
                }).ToList();
            //return new PagedResultDto<CommentDto>(comments, totalCount);
        }

        [Fact]
        public void Post()
        {
            Comment comment = new Comment()
            {
                Text = "😃😃😃😃"
            };
            _baseRepository.Insert(comment);
        }

        [Fact]
        public void GetAll()
        {
            CommentSearchDto commentSearchDto = new CommentSearchDto()
            {
                Count = 10,
                Page = 11
            };
            var comment0 = _baseRepository
                .Select
                .OrderByDescending(r => r.CreateTime)
                .Count(out long count1);

            var comments = _baseRepository
                 .Select
                 .OrderByDescending(r => r.CreateTime)
                 .Page(commentSearchDto.Page + 1, commentSearchDto.Count)
                 .ToList();
        }
    }
}
