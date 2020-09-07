using LinCms.Entities;
using System;
using System.Collections.Generic;

namespace LinCms.Cms.Permissions
{
    public class PermissionDto : EntityDto<long>
    {
        public PermissionDto(string name, string module, string router)
        {
            Name = name;
            Module = module;
            Router = router;
        }

        public string Name { get; set; }
        public string Module { get; set; }
        public string Router { get; set; }

    }

    public class TreePermissionDto
    {
        public string Rowkey { get; set; }
        public string Name { get; set; }
        public string Router { get; set; }
        public DateTime? CreateTime { get; set; }
        public List<TreePermissionDto> Children { get; set; }

    }
}
