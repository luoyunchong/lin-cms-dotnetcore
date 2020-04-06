using System;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Blog.Tags.Dtos
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
