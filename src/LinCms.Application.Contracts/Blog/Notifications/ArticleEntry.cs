using System;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Blog.Notifications;

public class ArticleEntry : EntityDto<Guid>
{
    public string Title { get; set; }

}