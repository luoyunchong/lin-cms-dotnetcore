using System;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities.Blog
{
    /// <summary>
    ///  频道标签
    /// </summary>
    [Table(Name = "blog_channel_tag")]
    public class ChannelTag : FullAuditEntity<Guid, long>
    {

        public ChannelTag()
        {
        }

        public ChannelTag(Guid tagId)
        {
            TagId = tagId;
        }

        public ChannelTag(Guid channelId, Guid tagId)
        {
            ChannelId = channelId;
            TagId = tagId;
        }

        /// <summary>
        /// 频道Id
        /// </summary>
        public Guid ChannelId { get; set; }

        /// <summary>
        /// 标签Id
        /// </summary>
        public Guid TagId { get; set; }

        [Navigate("ChannelId")]
        public virtual Channel Channel { get; set; }
        [Navigate("TagId")]
        public virtual Tag Tag { get; set; }
    }
}
