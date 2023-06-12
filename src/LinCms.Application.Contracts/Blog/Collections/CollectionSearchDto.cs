using LinCms.Data;
using System;

namespace LinCms.Blog.Collections;

public class CollectionSearchDto : PageDto
{
    public string Name { get; set; }

    public long? UserId { get; set; }
}