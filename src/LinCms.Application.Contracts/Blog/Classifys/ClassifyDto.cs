using System;
using LinCms.Entities;

namespace LinCms.Blog.Classifys
{
    public class ClassifyDto : EntityDto<Guid>, ICreateAuditEntity
    {
        public string Thumbnail { get; set; }
        public string ThumbnailDisplay { get; set; }
        public int SortCode { get; set; }
        public string ClassifyName { get; set; }
        public int ArticleCount { get; set; } = 0;
        public long CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
