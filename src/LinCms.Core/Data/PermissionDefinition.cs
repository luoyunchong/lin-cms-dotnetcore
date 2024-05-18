using System;

namespace LinCms.Data
{
    public class PermissionDefinition(string permission, string module, string router)
    {
        public string Permission { get; } = permission ?? throw new ArgumentNullException(nameof(permission));
        public string Module { get; } = module ?? throw new ArgumentNullException(nameof(module));
        public string Router { get; } = router ?? throw new ArgumentNullException(nameof(router));

        public override string ToString()
        {
            return base.ToString() + $" Permission:{Permission}、Module:{Module}、Router:{Router}";
        }
    }
}
