using System;

namespace LinCms.Application.Contracts.Blog.UserTags.Dtos
{
    public class UserTagDto
    {
        public Guid TagId { get; set; }
        public long CreateUserId { get; set; }
        public bool IsSubscribeed { get; set; }
    }
}
