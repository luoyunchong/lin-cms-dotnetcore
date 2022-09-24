using System;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities.Blog
{
    /// <summary>
    /// 用户点赞:点赞随笔、评论内容
    /// </summary>
    [Table(Name = "blog_user_like")]
    public class UserLike : Entity<Guid>, ICreateAuditEntity<long>
    {
        /// <summary>
        /// 点赞对象
        /// </summary>
        public Guid SubjectId { get; set; }

        /// <summary>
        /// 点赞类型 1 是随笔，2 是评论
        /// </summary>
        [Column(MapType = typeof(int))]
        public UserLikeSubjectType SubjectType { get; set; }

        public long? CreateUserId { get; set; }

        public DateTime CreateTime { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }

        [Navigate("SubjectId")]
        public virtual Comment Comment { get; set; }

        [Navigate("SubjectId")]
        public virtual Article Article { get; set; }
        public string CreateUserName { get; set; }
    }

    public enum UserLikeSubjectType
    {
        /// <summary>
        /// 点赞随笔
        /// </summary>
        UserLikeArticle = 1,
        /// <summary>
        /// 点赞评论
        /// </summary>
        UserLikeComment = 2
    }
}
