using System;

namespace LinCms.Blog.Tags
{
    public class UserTagDto
    {
        public Guid TagId { get; set; }
        public long CreateUserId { get; set; }
        public bool IsSubscribeed { get; set; }
    }
}
