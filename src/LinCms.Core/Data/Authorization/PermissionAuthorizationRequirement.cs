using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;

namespace LinCms.Data.Authorization
{
    public class ModuleAuthorizationRequirement : OperationAuthorizationRequirement
    {
        public ModuleAuthorizationRequirement(string module,string permission)
        {
            Module = module ?? throw new ArgumentNullException(nameof(module));
            Name = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        public string Module { get; set; }
    }
}
