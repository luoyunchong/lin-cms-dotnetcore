using System;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities.Blog;

/// <summary>
/// 用户文章收藏
/// </summary>
[Table(Name = "blog_article_collection")]
public class ArticleCollection : Entity<Guid>, ICreateAuditEntity<long>
{
    public Guid ArticleId { get; set; }
    public Guid CollectionId { get; set; }
    public long? CreateUserId { get; set; }
    public string CreateUserName { get; set; }
    public DateTime CreateTime { get; set; }
    
    public Article Article { get; set; }
    public Collection Collection { get; set; }
}