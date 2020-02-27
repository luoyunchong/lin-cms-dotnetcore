﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using LinCms.Application.Cms.Users;
using LinCms.Core.Entities;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LinCms.IdentityServer4.IdentityServer4
{
    public class LinCmsProfileService : IProfileService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        private readonly IUserService _userService;

        /// <summary>
        /// The claims factory.
        /// </summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="TestUserProfileService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="userService"></param>
        public LinCmsProfileService(ILogger<TestUserProfileService> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// 只要有关用户的身份信息单元被请求（例如在令牌创建期间或通过用户信息终点），就会调用此方法
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            var user = await _userService.GetUserAsync(r => r.Id == sub.ToLong());
            if (user == null)
            {
                _logger?.LogWarning("No user found matching subject Id: {0}", sub);
            }
            else
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.GivenName, user.Nickname ?? ""),
                    new Claim(ClaimTypes.Name, user.Username ?? ""),
                    new Claim(LinCmsClaimTypes.IsAdmin, user.IsAdmin().ToString()),
                    new Claim(ClaimTypes.Role,user.IsAdmin()?LinGroup.Admin:"")
                };

                user.LinGroups.ForEach(r =>
                 {
                     claims.Add(new Claim(LinCmsClaimTypes.Groups, r.Id.ToString()));
                 });

                context.IssuedClaims = claims;
            }
        }

        /// <summary>
        /// 验证用户是否有效 例如：token创建或者验证
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);

            var user = await _userService.GetUserAsync(r => r.Id == context.Subject.GetSubjectId().ToLong());
            context.IsActive = user.IsActive();
        }

    }
}
