using LinCms.Data;

namespace LinCms.Base.Localizations
{
    public class ResourceSearchDto : PageDto
    {
        public long CultureId { get; set; }

        public string Key { get; set; }
    }
}
