using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities
{
    /// <summary>
    /// 用户分组中间表
    /// </summary>
    [Table(Name = "lin_user_group")]
    public class LinUserGroup : Entity<long>
    {
        public LinUserGroup()
        {
        }
        public LinUserGroup(long userId, long groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }

        public long UserId { get; set; }

        public long GroupId { get; set; }

        [Navigate("UserId")]
        public LinUser LinUser { get; set; }

        [Navigate("GroupId")]
        public LinGroup LinGroup { get; set; }
    }
}
