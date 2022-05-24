using System;
using System.Collections.Generic;
using LinCms.Blog.Tags;
using LinCms.Cms.Users;
using LinCms.Common;
using LinCms.Entities;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Articles
{
    public class ArticleListDto : Entity<Guid>, ICreateAduitEntity
    {
        /// <summary>
        /// 技术频道Id
        /// </summary>
        public Guid? ChannelId { get; set; }
        /// <summary>
        /// 几小时/秒前
        /// </summary>
        public string TimeSpan => LinCmsUtils.GetTimeDifferNow(CreateTime.ToDateTime());

        private readonly DateTime _now = DateTime.Now;
        public bool IsNew => DateTime.Compare(_now.AddDays(-2), CreateTime.ToDateTime()) <= 0;

        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Excerpt { get; set; }
        public int ViewHits { get; set; }
        public int CommentQuantity { get; set; }
        public int LikesQuantity { get; set; }
        public string Thumbnail { get; set; }
        public string ThumbnailDisplay { get; set; }
        public bool IsAudit { get; set; }
        public bool Recommend { get; set; }
        public bool IsStickie { get; set; }
        public string Archive { get; set; }
        public ArticleType ArticleType { get; set; }
        public long CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Author { get; set; }
        public bool IsLiked { get; set; }

        public OpenUserDto UserInfo { get; set; }

        public List<TagDto> Tags { get; set; }
    }
}
