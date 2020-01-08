using System;

namespace LinCms.Application.Contracts.v1.UserTags
{
    public class UserTagDto
    {
        public Guid TagId { get; set; }
        public long CreateUserId { get; set; }
        public bool IsSubscribeed { get; set; }
    }
}
