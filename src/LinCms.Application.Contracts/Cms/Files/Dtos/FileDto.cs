using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Files.Dtos
{
    public class FileDto:Entity
    {
        public string Key { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
