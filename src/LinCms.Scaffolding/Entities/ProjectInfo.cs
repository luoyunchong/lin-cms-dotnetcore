using System.Linq;

namespace LinCms.Scaffolding.Entities
{
    public class ProjectInfo
    {
        public ProjectInfo(string baseDirectory, string fullName)
        {
            BaseDirectory = baseDirectory;
            FullName = fullName;
        }

        public string BaseDirectory { get; }
        public string FullName { get; }
        public string Name => FullName.Split('.').Last();
    }
}
