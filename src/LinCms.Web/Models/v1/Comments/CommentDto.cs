using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.Comments
{
    public class CommentDto:EntityDto
    {
        public int? PId { get; set; }
        public string PName { get; set; }
        public string Text { get; set; }
        public string Ip { get; set; }
        public string Agent { get; set; }
        public string System { get; set; }
        public string GeoPosition { get; set; }
        public string AuName { get; set; }
        public string AuEmail { get; set; }
        public string UserHost { get; set; }
        public bool? IsAudited { get; set; }
        public string Avatar { get; set; }
        public int? ArticleId { get; set; }
    }
}
