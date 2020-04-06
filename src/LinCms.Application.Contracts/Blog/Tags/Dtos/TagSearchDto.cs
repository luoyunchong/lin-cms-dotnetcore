using System;
using System.Collections.Generic;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Blog.Tags.Dtos
{
    public class TagSearchDto:PageDto
    {
        public List<Guid> TagIds { get; set; }
        public string TagName { get; set; }

        public bool? Status { get; set; }
    }
}
