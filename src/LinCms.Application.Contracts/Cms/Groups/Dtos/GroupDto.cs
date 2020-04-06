using System.Collections.Generic;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Groups.Dtos
{
    public class GroupDto:Entity
    {
        public List<LinPermission> Permissions { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public bool IsStatic { get; set; }

    }
}
