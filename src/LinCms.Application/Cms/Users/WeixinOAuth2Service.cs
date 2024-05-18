using AspNet.Security.OAuth.Weixin;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Common;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.IRepositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LinCms.Cms.Users;

[DisableConventionalRegistration]
public class WeixinOAuth2Service(IAuditBaseRepository<LinUserIdentity> userIdentityRepository,
        IUserRepository userRepository)
    : OAuthService(userIdentityRepository), IOAuth2Service
{
    public override async Task<long> SaveUserAsync(ClaimsPrincipal principal, string unionId)
    {
        LinUserIdentity linUserIdentity = await userIdentityRepository.Where(r => r.IdentityType == LinUserIdentity.Weixin && r.Credential == unionId).FirstAsync();

        long userId = 0;
        if (linUserIdentity == null)
        {

            string gender = principal.FindFirst(ClaimTypes.Gender)?.Value;
            string nickname = principal.FindFirst(ClaimTypes.Name)?.Value;

            string openId = principal.FindFirst(WeixinAuthenticationConstants.Claims.OpenId)?.Value;

            string avatarUrl = principal.FindFirst(WeixinAuthenticationConstants.Claims.HeadImgUrl)?.Value;

            LinUser user = new()
            {
                Active = UserStatus.Active,
                Avatar = avatarUrl,
                LastLoginTime = DateTime.Now,
                Email = "",
                Introduction = "",
                LinUserGroups = new List<LinUserGroup>()
                {
                    new()
                    {
                        GroupId = LinConsts.Group.User
                    }
                },
                Nickname = nickname,
                Username = "",
                BlogAddress = "",
                LinUserIdentitys = new List<LinUserIdentity>()
                {
                    new(LinUserIdentity.Weixin,nickname,unionId,DateTime.Now)
                }
            };
            await userRepository.InsertAsync(user);
            userId = user.Id;
        }
        else
        {
            if (linUserIdentity.CreateUserId != null) userId = linUserIdentity.CreateUserId.Value;
        }

        return userId;
    }

}