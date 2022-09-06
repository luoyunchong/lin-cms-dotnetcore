using System;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities.Blog;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Blog.Comments;

public interface ICommentService
{
    /// <summary>
    /// 随笔下的评论列表
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

    /// <summary>
    /// 发表评论
    /// </summary>
    /// <param name="createCommentDto"></param>
    /// <returns></returns>
    Task CreateAsync(CreateCommentDto createCommentDto);

    /// <summary>
    /// 删除评论并同步随笔数量
    /// </summary>
    /// <param name="comment"></param>
    Task DeleteAsync(Comment comment);

    /// <summary>
    /// 删除评论,先查一次
    /// </summary>
    /// <param name="id">评论表Id</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// 只删除自己的评论
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteMyComment(Guid id);

    /// <summary>
    /// 修改评论的点赞的数量
    /// </summary>
    /// <param name="subjectId"></param>
    /// <param name="likesQuantity"></param>
    /// <returns></returns>
    Task UpdateLikeQuantityAysnc(Guid subjectId, int likesQuantity);
}