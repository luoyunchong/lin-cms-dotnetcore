using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Aop.Filter;
using LinCms.Blog.Comments;
using LinCms.Data;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 随笔评论
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/comments")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly IAuditBaseRepository<Comment> _commentRepository;
    private readonly ICommentService _commentService;
    private readonly IAuditBaseRepository<Article> _articleRepository;

    public CommentController(
        IAuditBaseRepository<Comment> commentAuditBaseRepository,
        ICommentService commentService, IAuditBaseRepository<Article> articleRepository)
    {
        _commentRepository = commentAuditBaseRepository;
        _commentService = commentService;
        _articleRepository = articleRepository;
    }

    /// <summary>
    /// 评论分页列表页,当随笔Id有值时，查询出此随笔的评论
    /// </summary>
    /// <param name="commentSearchDto"></param>
    /// <returns></returns>
    [HttpGet("public")]
    [AllowAnonymous]
    public Task<PagedResultDto<CommentDto>> GetListByArticleAsync([FromQuery] CommentSearchDto commentSearchDto)
    {
        return _commentService.GetListByArticleAsync(commentSearchDto);
    }

    /// <summary>
    /// 后台权限-评论列表页
    /// </summary>
    /// <param name="commentSearchDto"></param>
    /// <returns></returns>
    [HttpGet]
    [LinCmsAuthorize("评论列表", "评论")]
    public Task<PagedResultDto<CommentDto>> GetListAsync([FromQuery] CommentSearchDto commentSearchDto)
    {
        return _commentService.GetListAsync(commentSearchDto);
    }

    /// <summary>
    /// 后台权限-删除评论
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("cms/{id}")]
    [LinCmsAuthorize("删除评论", "评论")]
    public async Task<UnifyResponseDto> DeleteAsync(Guid id)
    {
        await _commentService.DeleteAsync(id);
        return UnifyResponseDto.Success();
    }

    /// <summary>
    /// 用户仅可删除自己的评论
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<UnifyResponseDto> DeleteMyComment(Guid id)
    {
        await _commentService.DeleteMyComment(id);
        return UnifyResponseDto.Success();
    }

    /// <summary>
    /// 用户评论
    /// </summary>
    /// <param name="createCommentDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateCommentDto createCommentDto)
    {
        await _commentService.CreateAsync(createCommentDto);
        return UnifyResponseDto.Success("评论成功");
    }

    /// <summary>
    /// 审核评论-拉黑/取消拉黑
    /// </summary>
    /// <param name="id">审论Id</param>
    /// <param name="isAudit"></param>
    /// <returns></returns>
    [LinCmsAuthorize("审核评论", "评论")]
    [HttpPut("{id}")]
    public async Task<UnifyResponseDto> UpdateAsync(Guid id, bool isAudit)
    {
        Comment comment =await _commentRepository.Select.Where(r => r.Id == id).FirstAsync();
        if (comment == null)
        {
            throw new LinCmsException("没有找到相关评论");
        }

        comment.IsAudit = isAudit;
        await _commentRepository.UpdateAsync(comment);
        return UnifyResponseDto.Success();
    }

    /// <summary>
    /// 评论-校正评论量,subjectType(1：随笔)
    /// </summary>
    /// <param name="subjectId"></param>
    /// <param name="subjectType"></param>
    /// <returns></returns>
    [LinCmsAuthorize("校正评论量", "评论")]
    [HttpPut("{subjectId}/type/${subject_type}")]
    public UnifyResponseDto CorrectedComment(Guid subjectId, int subjectType)
    {
        long count = _commentRepository.Select.Where(r => r.SubjectId == subjectId).Count();

        switch (subjectType)
        {
            case 1:
                _articleRepository.UpdateDiy.Set(r => r.CommentQuantity, count).Where(r => r.Id == subjectId)
                    .ExecuteAffrows();
                break;
        }

        return UnifyResponseDto.Success();
    }

}