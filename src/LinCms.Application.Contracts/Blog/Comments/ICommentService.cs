using System;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities.Blog;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Blog.Comments
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
        Task CreateAsync( CreateCommentDto createCommentDto);
        Task DeleteAsync(Comment comment);
        Task DeleteAsync(Guid id);
        Task DeleteMyComment(Guid id);
        Task UpdateLikeQuantityAysnc(Guid subjectId, int likesQuantity);
    }
}