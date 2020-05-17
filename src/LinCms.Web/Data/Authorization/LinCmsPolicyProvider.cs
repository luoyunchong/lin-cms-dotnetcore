using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

namespace LinCms.Web.Data.Authorization
{
    public class LinCmsPolicyProvider:DefaultAuthorizationPolicyProvider,IAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;
        private readonly IPermissionService _permissionService;

        public LinCmsPolicyProvider(IOptions<AuthorizationOptions> options, IPermissionService permissionService): base(options)
        {
            _permissionService = permissionService;
            _options = options.Value;
        }


        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
            {
                return policy;
            }

            var permission =await _permissionService.GetAsync(policyName);
            if (permission != null)
            {
                //TODO: Optimize & Cache!
                var policyBuilder = new AuthorizationPolicyBuilder(Array.Empty<string>());
                policyBuilder.Requirements.Add(new OperationAuthorizationRequirement(){Name = policyName});
                return policyBuilder.Build();
            }

            return null;
        }

    }
}