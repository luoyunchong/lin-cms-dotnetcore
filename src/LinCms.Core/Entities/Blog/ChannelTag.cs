using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Blog
{
    [Table(Name = "blog_channel_tag")]
    public class ChannelTag : Entity<Guid>
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

        public Guid ChannelId { get; set; }
        public Guid TagId { get; set; }

        [Navigate("ChannelId")]
        public virtual Channel Channel { get; set; }
        [Navigate("TagId")]
        public virtual Tag Tag { get; set; }
    }
}
