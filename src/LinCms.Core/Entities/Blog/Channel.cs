using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Blog
{
    /// <summary>
    /// 技术频道，官方分类。标签的分类。
    /// </summary>
    [Table(Name = "blog_channel")]
    public class Channel : FullAuditEntity<Guid>
    {
        /// <summary>
        /// 封面图
        /// </summary>
        [Column(StringLength = 100)]
        public string Thumbnail { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortCode { get; set; }

        /// <summary>
        /// 技术频道名称
        /// </summary>
        [Column(StringLength = 50)]

        public string ChannelName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Column(StringLength = 50)]
        public string ChannelCode { get; set; }

        /// <summary>
        /// 备注描述
        /// </summary>
        [Column(StringLength = 500)]
        public string Remark { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Status { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual List<Article> Articles { get; set; }

    }
}
