using System;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.Tags
{
    public class TagDto:EntityDto
    {
        public string Thumbnail { get; set; }
        public string ThumbnailDisplay { get; set; }
        public string TagName { get; set; }
        public long? CreateUserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Alias { get; set; }
     
    }
}
