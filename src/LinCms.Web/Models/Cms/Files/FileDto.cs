using LinCms.Zero.Domain;

namespace LinCms.Web.Models.Cms.Files
{
    public class FileDto:Entity
    {
        public string Key { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
