using System;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Comments.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Application.Contracts.Blog.Comments
{
    public interface ICommentService
    {
        /// <summary>
        /// 文章下的评论列表
        /// </summary>
        /// <param name="commentSearchDto"></param>
        /// <returns></returns>
        Task<PagedResultDto<CommentDto>> GetListByArticleAsync(CommentSearchDto commentSearchDto);

        /// <summary>
        /// 评论分页项
        /// </summary>
        /// <param name="commentSearchDto"></param>
        /// <returns></returns>
        Task<PagedResultDto<CommentDto>> GetListAsync([FromQuery] CommentSearchDto commentSearchDto);

        Task DeleteAsync(Comment comment);
        Task UpdateLikeQuantityAysnc(Guid subjectId, int likesQuantity);
    }
}