using System;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.Classifys
{
    public class ClassifyDto : EntityDto<Guid>, ICreateAduitEntity
    {
        public string ClassifyCode { get; set; }
        public int SortCode { get; set; }
        public string ClassifyName { get; set; }
        public long? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
