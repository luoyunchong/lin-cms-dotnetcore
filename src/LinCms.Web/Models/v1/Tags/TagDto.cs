using System;
using FreeSql.DataAnnotations;
using LinCms.Web.Models.Cms.Users;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.Tags
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
        public bool Status { get; set; }

        public virtual OpenUserDto OpenUserDto { get; set; }
        public LinUser LinUser { get; set; }

    }
}
