using System.Reflection;

namespace LinCms.Domain
{
    public class LinAuth : Entity
    {
        public int GroupId { get; set; }
        public string Auth { get; set; }
        public string Module { get; set; }
    }
}
