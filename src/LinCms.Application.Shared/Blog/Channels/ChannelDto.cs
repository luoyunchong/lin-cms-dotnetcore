using System;
using System.Collections.Generic;
using System.Linq;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Blog.Channels
{
    public class ChannelDto:Entity<Guid>
    {
        /// <summary>
        /// 封面图
        /// </summary>
        public string Thumbnail { get; set; }

        public string ThumbnailDisplay { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 技术频道名称
        /// </summary>

        public string ChannelName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string ChannelCode { get; set; }

        /// <summary>
        /// 备注描述
        /// </summary>
        public string Remark { get; set; }

        public bool Status { get; set; }


        public List<Guid> TagIds
        {
            get { return this.Tags.Select(r => r.Id).ToList(); }
        }

        public List<TagDto> Tags { get; set; }
    }
}
