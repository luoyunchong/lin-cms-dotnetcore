using System;
using LinCms.Entities;

namespace LinCms.Blog.Notifications
{
    public class ArticleEntry : EntityDto<Guid>
    {
        public string Title { get; set; }

    }
}
