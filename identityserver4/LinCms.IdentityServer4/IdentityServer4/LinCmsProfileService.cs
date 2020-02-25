using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using LinCms.Application.Cms.Users;
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
        /// Initializes a new instance of the <see cref="TestUserProfileService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
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
        public virtual Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(_logger);

            context.IssuedClaims = context.Subject.Claims.ToList();

            context.LogIssuedClaims(_logger);

            return Task.CompletedTask;
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
