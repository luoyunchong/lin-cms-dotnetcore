using LinCms.Data;

namespace LinCms.Blog.Classifys;

public class ClassifySearchDto : PageDto
{
    public long? UserId { get; set; }
    public string ClassifyName { get; set; }
}