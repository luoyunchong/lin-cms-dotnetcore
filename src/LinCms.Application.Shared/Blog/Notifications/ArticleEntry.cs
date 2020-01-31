using System;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Blog.Notifications
{
    public class ArticleEntry:EntityDto<Guid>
    {
        public string Title { get; set; }

    }
}
