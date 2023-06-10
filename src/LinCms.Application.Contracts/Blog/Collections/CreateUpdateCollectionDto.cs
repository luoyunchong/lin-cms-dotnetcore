using JetBrains.Annotations;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Collections;

public class CreateUpdateCollectionDto
{
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [CanBeNull]
    public string Remark { get; set; }

    /// <summary>
    /// 公开 当其他人关注此收藏集后不可再更改为隐私
    /// 隐私 仅自己可见此收藏集
    /// </summary>
    public PrivacyType PrivacyType { get; set; }
}