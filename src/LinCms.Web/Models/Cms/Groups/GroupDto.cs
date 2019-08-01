using System.Collections.Generic;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.Cms.Groups
{
    public class GroupDto:Entity
    {
        public List<IDictionary<string,object>> Auths { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
    }
}
