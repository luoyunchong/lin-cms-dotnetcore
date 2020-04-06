using System;
using System.Collections.Generic;

namespace LinCms.Application.Contracts.Blog.Channels.Dtos
{
    public class CreateUpdateChannelDto
    {
        /// <summary>
        /// 封面图
        /// </summary>
        public string Thumbnail { get; set; }
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
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Status { get; set; }

        public List<Guid> TagIds { get; set; }

    }
}
