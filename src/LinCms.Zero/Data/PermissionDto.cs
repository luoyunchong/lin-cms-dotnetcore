using System;

namespace LinCms.Core.Data
{
    public class PermissionDto
    {
        public PermissionDto(string permission, string module, string router)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
            Module = module ?? throw new ArgumentNullException(nameof(module));
            Router = router ?? throw new ArgumentNullException(nameof(router));
        }

        public string Permission { get; }
        public string Module { get;  }
        public string Router { get; }
        public override string ToString()
        {
            return base.ToString() +$" Permission:{Permission}、Module:{Module}、Router:{Router}";
        }
    }
}
