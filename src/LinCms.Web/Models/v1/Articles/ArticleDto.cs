using LinCms.Zero.Common;
using LinCms.Zero.Domain;
using LinCms.Zero.Domain.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using LinCms.Web.Models.Cms.Users;
using LinCms.Web.Models.v1.Tags;

namespace LinCms.Web.Models.v1.Articles
{
    public class ArticleDto : Entity<Guid>, ICreateAduitEntity
    {
        /// <summary>
        /// 类别Id
        /// </summary>
        public Guid? ClassifyId { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>        
        public string ClassifyName { get; set; }
        /// <summary>
        /// 几小时/秒前
        /// </summary>
        public string TimeSpan => LinCmsUtils.GetTimeDifferNow(this.CreateTime.ToDateTime());

        private readonly DateTime _now = DateTime.Now;
        public bool IsNew => DateTime.Compare(_now.AddDays(-2), this.CreateTime.ToDateTime()) <= 0;

        public string Content { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Source { get; set; }
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
        public int Editor { get; set; } = 1;
        public long? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Author { get; set; }

        public bool IsLiked { get; set; }
        public bool IsComment { get; set; }
        public bool IsCollect { get; set; }

        public OpenUserDto UserInfo { get; set; }

        public List<Guid> TagIds
        {
            get { return this.Tags.Select(r => r.Id).ToList(); }
        }

        public List<TagDto> Tags { get; set; }
    }
}
