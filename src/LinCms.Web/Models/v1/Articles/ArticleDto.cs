using LinCms.Zero.Common;
using LinCms.Zero.Domain;
using System;
namespace LinCms.Web.Models.v1.Articles
{
    public class ArticleDto : Entity, ICreateAduitEntity
    {

        //类别Id
        public int? FId { get; set; }
        //类别编码  
        public string TypeCode { get; set; }
        //类别名称 
        public string TypeName { get; set; }
        //发布人
        public string NickName { get; set; }
        //几小时/秒前
        public string TimeSpan
        {
            get
            {
                return LinCmsUtils.GetTimeDifferNow(this.CreateTime.ToDateTime());
            }
        }

        DateTime now = DateTime.Now;
        public bool IsNew
        {
            get
            {
                return DateTime.Compare(now.AddDays(-2), this.CreateTime.ToDateTime()) > 0 ? false : true; ;
            }
        }

        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Source { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public int ViewHits { get; set; }
        public int CommentQuantity { get; set; }
        public int PointQuantity { get; set; }
        public string Thumbnail { get; set; }
        public bool IsAudit { get; set; }
        public bool Recommend { get; set; }
        public bool IsStickie { get; set; }
        public string Archive { get; set; }
        public string ArticleType { get; set; }
        public int Editor { get; set; } = 1;
        public long? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
