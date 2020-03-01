using System;
using System.Collections.Generic;
using System.Text;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Permissions
{
    public class PermissionDto:EntityDto<long>
    {
        public PermissionDto(string name, string module)
        {
            Name = name;
            Module = module;
        }

        public string Name { get; set; }
        public string Module { get; set; }
    }
}
