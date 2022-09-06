using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.QQ;
using LinCms.Common;
using LinCms.Data.Enums;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities;
using LinCms.IRepositories;

namespace LinCms.Cms.Users;

[DisableConventionalRegistration]
public class QQOAuth2Service : OAuthService, IOAuth2Service
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditBaseRepository<LinUserIdentity> _userIdentityRepository;

    public QQOAuth2Service(IAuditBaseRepository<LinUserIdentity> userIdentityRepository, IUserRepository userRepository) : base(userIdentityRepository)
    {
        _userIdentityRepository = userIdentityRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// qq快速登录的信息，唯一值openid,昵称(nickname)，性别(gender)，picture（30像素）,picture_medium(50像素），picture_full 100 像素，avatar（40像素），avatar_full(100像素）
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="openId"></param>
    /// <returns></returns>
    public override async Task<long> SaveUserAsync(ClaimsPrincipal principal, string openId)
    {

        LinUserIdentity linUserIdentity = await _userIdentityRepository.Where(r => r.IdentityType == LinUserIdentity.QQ && r.Credential == openId).FirstAsync();

        long userId = 0;
        if (linUserIdentity == null)
        {

            string nickname = principal.FindFirst(ClaimTypes.Name)?.Value ?? "默认昵称";
            string gender = principal.FindFirst(ClaimTypes.Gender)?.Value;
            string picture = principal.FindFirst(QQAuthenticationConstants.Claims.PictureUrl)?.Value;
            string picture_medium = principal.FindFirst(QQAuthenticationConstants.Claims.PictureMediumUrl)?.Value;
            string picture_full = principal.FindFirst(QQAuthenticationConstants.Claims.PictureFullUrl)?.Value;
            string avatarUrl = principal.FindFirst(QQAuthenticationConstants.Claims.AvatarUrl)?.Value;
            string avatarFullUrl = principal.FindFirst(QQAuthenticationConstants.Claims.AvatarFullUrl)?.Value;

            LinUser user = new()
            {
                Active = UserStatus.Active,
                Avatar = avatarFullUrl,
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
                    new(LinUserIdentity.QQ, nickname, openId,DateTime.Now)
                }
            };
            await _userRepository.InsertAsync(user);
            userId = user.Id;
        }
        else
        {
            if (linUserIdentity.CreateUserId != null) userId = (long)linUserIdentity.CreateUserId;
        }

        return userId;
    }
}