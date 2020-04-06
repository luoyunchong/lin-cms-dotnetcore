using System;
using System.Collections.Generic;
using System.Linq;
using LinCms.Application.Contracts.Blog.Classifys.Dtos;
using LinCms.Application.Contracts.Blog.Tags.Dtos;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using LinCms.Core.Common;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.Articles.Dtos
{
    public class ArticleDto : Entity<Guid>, ICreateAduitEntity
    {
        /// <summary>
        /// 分类专栏Id
        /// </summary>
        public Guid? ClassifyId { get; set; }
        /// <summary>
        /// 技术频道Id
        /// </summary>
        public Guid? ChannelId { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>        
        public ClassifyDto Classify { get; set; }
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
        public long CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Author { get; set; }

        public bool IsLiked { get; set; }
        public bool IsComment { get; set; }
        public bool IsCollect { get; set; }

        public OpenUserDto UserInfo { get; set; }

        public long WordNumber { get; set; }

        public long ReadingTime { get; set; }

        public List<Guid> TagIds
        {
            get { return this.Tags.Select(r => r.Id).ToList(); }
        }

        public List<TagListDto> Tags { get; set; }
    }
}
