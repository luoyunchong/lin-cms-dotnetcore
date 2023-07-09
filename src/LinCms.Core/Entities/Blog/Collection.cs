using System;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;
using JetBrains.Annotations;

namespace LinCms.Entities.Blog;

/// <summary>
/// 收藏集
/// </summary>
[Table(Name = "blog_collection")]
public class Collection : FullAuditEntity<Guid, long>
{
    /// <summary>
    /// 名称 
    /// </summary>
    [Column(StringLength = 50)]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [Column(StringLength = 200)]
    [CanBeNull]
    public string Remark { get; set; }

    /// <summary>
    /// 收藏数量 
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 公开 当其他人关注此收藏集后不可再更改为隐私
    /// 隐私 仅自己可见此收藏集
    /// </summary>
    public PrivacyType PrivacyType { get; set; }

    public void UpdateQuantity(int quantity)
    {
        if (quantity < 0 && Quantity < -quantity) return;
        Quantity += quantity;
    }
}

public enum PrivacyType
{
    /// <summary>
    /// 公开可见
    /// </summary>
    Public = 0,

    /// <summary>
    /// 仅自己可见
    /// </summary>
    VisibleOnlyMySelf = 1
}