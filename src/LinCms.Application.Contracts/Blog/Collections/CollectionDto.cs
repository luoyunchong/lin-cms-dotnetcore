using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Entities.Blog;
using System;

namespace LinCms.Blog.Collections;

public class CollectionDto : EntityDto<Guid>
{
    /// <summary>
    /// 名称 
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 公开 当其他人关注此收藏集后不可再更改为隐私
    /// 隐私 仅自己可见此收藏集
    /// </summary>
    public PrivacyType PrivacyType { get; set; }
}
