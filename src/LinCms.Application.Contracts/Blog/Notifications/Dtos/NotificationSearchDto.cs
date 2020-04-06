using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Blog.Notifications.Dtos
{
    public class NotificationSearchDto : PageDto
    {
        public NotificationSearchType? NotificationSearchType { get; set; }
    }

    public enum NotificationSearchType
    {
        /// <summary>
        /// 用户点赞随笔、点赞评论
        /// </summary>
        UserLike= 0,
        /// <summary>
        /// 用户评论随笔、回复别人的评论
        /// </summary>
        UserComment = 1,
        /// <summary>
        /// 用户关注用户
        /// </summary>
        UserLikeUser = 2
    }
}
