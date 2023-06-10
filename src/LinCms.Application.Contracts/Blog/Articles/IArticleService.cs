using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Data;

namespace LinCms.Blog.Articles;

public interface IArticleService
{
    #region CRUD
    Task<Guid> CreateAsync(CreateUpdateArticleDto createArticle);

    Task UpdateAsync(Guid id, CreateUpdateArticleDto updateArticleDto);

    Task<PagedResultDto<ArticleListDto>> GetArticleAsync(ArticleSearchDto searchDto);

    Task DeleteAsync(Guid id);

    Task<ArticleDto> GetAsync(Guid id);
    #endregion

    /// <summary>
    /// 得到我关注的人发布的随笔
    /// </summary>
    /// <param name="pageDto"></param>
    /// <returns></returns>
    Task<PagedResultDto<ArticleListDto>> GetSubscribeArticleAsync(PageDto pageDto);

    /// <summary>
    /// 更新随笔点赞量
    /// </summary>
    /// <param name="subjectId"></param>
    /// <param name="likesQuantity"></param>
    /// <returns></returns>
    Task UpdateLikeQuantityAysnc(Guid subjectId, int likesQuantity);

    Task UpdateCollectQuantityAysnc(Guid articleId, int collectQuantity);

    /// <summary>
    /// 修改随笔是否允许其他人评论
    /// </summary>
    /// <param name="id"></param>
    /// <param name="commentable"></param>
    /// <returns></returns>
    Task UpdateCommentable(Guid id, bool commentable);
}