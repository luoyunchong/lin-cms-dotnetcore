namespace LinCms.Blog.AuthorCenter;

/// <summary>
///  文章统计Card
/// </summary>
public class ArticleCardDto
{
    /// <summary>
    /// 总文章数
    /// </summary>
    public long AllArticle { get; set; }

    /// <summary>
    /// 文章收藏数
    /// </summary>
    public long AllArticleCollect { get; set; }

    /// <summary>
    /// 文章评论数
    /// </summary>
    public long AllArticleComment { get; set; }

    /// <summary>
    /// 文章点赞数
    /// </summary>
    public long AllArticleStar { get; set; }

    /// <summary>
    /// 文章阅读数
    /// </summary>
    public long AllArticleView { get; set; }
}