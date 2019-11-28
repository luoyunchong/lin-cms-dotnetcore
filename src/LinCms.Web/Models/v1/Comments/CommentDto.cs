using System;
using System.Collections.Generic;
using AutoMapper;
using LinCms.Web.Models.Cms.Users;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.Comments
{
    public class CommentDto:EntityDto<Guid>,ICreateAduitEntity
    {
        /// <summary>
        /// 回复评论Id
        /// </summary>
        public Guid? RespId { get; set; }
        /// <summary>
        /// 回复的文本内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 关联随笔id
        /// </summary>
        public Guid? SubjectId { get; set; }

        public long? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 评论的用户
        /// </summary>
        public OpenUserDto UserInfo { get; set; }

        /// <summary>
        /// 回复的用户
        /// </summary>
        public OpenUserDto RespUserInfo { get; set; }

        public bool IsLiked { get; set; }

        public bool? IsAudit { get; set; }

        public int LikesQuantity { get; set; }

        public Guid? RootCommentId { get; set; }
        /// <summary>
        /// 最新的二条回复
        /// </summary>
        public  List<CommentDto> TopComment { get; set; }


    }

}
