using System;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Blog
{
    [Table(Name = "blog_user_like")]
    public class UserLike : Entity<Guid>, ICreateAduitEntity
    {
        /// <summary>
        /// 点赞对象
        /// </summary>
        public Guid SubjectId { get; set; }

        /// <summary>
        /// 点赞类型 1 是文章，2 是评论
        /// </summary>
        public int SubjectType { get; set; }
        public long CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }


        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }

        [Navigate("SubjectId")]
        public virtual Comment Comment { get; set; }

        [Navigate("SubjectId")]
        public virtual Article Article { get; set; }
    }
}
