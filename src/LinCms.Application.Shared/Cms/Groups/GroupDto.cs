using System.Collections.Generic;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Groups
{
    public class GroupDto:Entity
    {
        public List<IDictionary<string,object>> Permissions { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public bool IsStatic { get; set; }

    }
}
