using System;
using System.Collections.Generic;
using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Blog.Tags;

namespace LinCms.Blog.Channels;

public class NavChannelListDto : Entity<Guid>
{
    /// <summary>
    /// 技术频道名称
    /// </summary>

    public string ChannelName { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string ChannelCode { get; set; }


    public List<TagDto> Tags { get; set; }
}