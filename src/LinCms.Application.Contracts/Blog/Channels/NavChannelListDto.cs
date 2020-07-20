using System;
using System.Collections.Generic;
using LinCms.Blog.Tags;
using LinCms.Entities;

namespace LinCms.Blog.Channels
{
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
}
