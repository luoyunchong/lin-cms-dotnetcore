using System;

namespace LinCms.Data
{
    public class PermissionDefinition(string permission, string module, string router)
    {
        public string Permission { get; set; } = permission ?? throw new ArgumentNullException(nameof(permission));
        public string Module { get; set; } = module ?? throw new ArgumentNullException(nameof(module));
        public string Router { get; set; } = router ?? throw new ArgumentNullException(nameof(router));

        public override string ToString()
        {
            return base.ToString() + $" Permission:{Permission}、Module:{Module}、Router:{Router}";
        }
    }
}
