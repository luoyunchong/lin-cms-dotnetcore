using System;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.v1.Tags
{
    public class TagDto:EntityDto<Guid>
    {
        public string Thumbnail { get; set; }
        public string ThumbnailDisplay { get; set; }
        public string TagName { get; set; }
        public long CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Alias { get; set; }
        public int ArticleCount { get; set; }
        public int SubscribersCount { get; set; }
        public bool Status { get; set; }
        public  bool IsSubscribe { get; set; }

    }
}
