using System;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Blog.Tags;

public class TagDto : EntityDto<Guid>
{
    public TagDto(Guid id, string tagName)
    {
        Id = id;
        TagName = tagName;
    }

    public string TagName { get; set; }
}