using System;
using System.Collections.Generic;
using LinCms.Data;

namespace LinCms.Blog.Tags;

public class TagSearchDto : PageDto
{
    public List<Guid> TagIds { get; set; }
    public string TagName { get; set; }

    public bool? Status { get; set; }
}