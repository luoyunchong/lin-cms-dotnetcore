using System;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Blog.Articles.Dtos
{
    public class ArticleSearchDto:PageDto
    {
        public Guid? ClassifyId { get; set; }
        public Guid? ChannelId { get; set; }
        public Guid? TagId { get; set; }
        public string Title { get; set; }
        public long? UserId { get; set; }

        public override string ToString()
        {
            return $"{ClassifyId}-{ChannelId}-{TagId}-{Title}-{UserId}";
        }

    }
}
