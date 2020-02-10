﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.Extensions.Logging;

namespace LinCms.IdentityServer4.IdentityServer4
{
    public class LinCmsProfileService : IProfileService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        private readonly IFreeSql _freeSql;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestUserProfileService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="freeSql"></param>
        public LinCmsProfileService(ILogger<TestUserProfileService> logger, IFreeSql freeSql)
        {
            _logger = logger;
            _freeSql = freeSql;
        }

        /// <summary>
        /// 只要有关用户的身份信息单元被请求（例如在令牌创建期间或通过用户信息终点），就会调用此方法
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(_logger);

            List<Claim> claims = context.Subject.Claims.ToList();
            context.IssuedClaims = claims.ToList();

            context.LogIssuedClaims(_logger);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 验证用户是否有效 例如：token创建或者验证
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);

            //var user = Users.FindBySubjectId(context.Subject.GetSubjectId());
            context.IsActive = /*user?.IsActive == */true;

            return Task.CompletedTask;
        }
    }
}
