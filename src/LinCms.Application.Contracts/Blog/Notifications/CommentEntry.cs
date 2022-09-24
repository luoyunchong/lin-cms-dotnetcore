using System;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Blog.Notifications;

public class CommentEntry : EntityDto<Guid>
{
    public Guid? RespId { get; set; }
    public string Text { get; set; }
}