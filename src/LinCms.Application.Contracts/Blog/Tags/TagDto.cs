using System;
using LinCms.Entities;

namespace LinCms.Blog.Tags
{
    public class TagDto : EntityDto<Guid>
    {
        public TagDto(Guid id, string tagName)
        {
            Id = id;
            TagName = tagName;
        }

        public string TagName { get; set; }
    }
}
