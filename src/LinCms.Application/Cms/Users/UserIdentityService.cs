using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using AspNet.Security.OAuth.QQ;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Aop;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Cms.Users
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IFreeSql _freeSql;
        private readonly IUserRepository _userRepository;
        public UserIdentityService(IFreeSql freeSql, IUserRepository userRepository)
        {
            _freeSql = freeSql;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 记录授权成功后的信息
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<long> SaveGitHubAsync(ClaimsPrincipal principal, string openId)
        {
            string email = principal.FindFirst(ClaimTypes.Email)?.Value;
            string name = principal.FindFirst(ClaimTypes.Name)?.Value;
            string gitHubName = principal.FindFirst(GitHubAuthenticationConstants.Claims.Name)?.Value;
            string gitHubApiUrl = principal.FindFirst(GitHubAuthenticationConstants.Claims.Url)?.Value;
            string avatarUrl = principal.FindFirst(LinConsts.Claims.AvatarUrl)?.Value;
            string bio = principal.FindFirst(LinConsts.Claims.BIO)?.Value;
            string blogAddress = principal.FindFirst(LinConsts.Claims.BlogAddress)?.Value;
            Expression<Func<LinUserIdentity, bool>> expression = r => 
                r.IdentityType == LinUserIdentity.GitHub&& r.Credential == openId;

            LinUserIdentity linUserIdentity =await _freeSql.Select<LinUserIdentity>().Where(expression).FirstAsync();

            long userId = 0;
            if (linUserIdentity == null)
            {
                LinUser user = new LinUser
                {
                    Admin = (int)UserAdmin.Common,
                    Active = (int)UserActive.Active,
                    Avatar = avatarUrl,
                    CreateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    Email = email,
                    Introduction = bio,
                    LinUserGroups = new List<LinUserGroup>()
                    {
                        new LinUserGroup()
                        {
                            GroupId = LinConsts.Group.User
                        }
                    },
                    Nickname = gitHubName,
                    Username = "",
                    BlogAddress = blogAddress,
                    LinUserIdentitys = new List<LinUserIdentity>()
                    {
                        new LinUserIdentity
                        {
                            CreateTime = DateTime.Now,
                            Credential = openId,
                            IdentityType = LinUserIdentity.GitHub,
                            Identifier = name,
                        }
                    }
                };
                await _userRepository.InsertAsync(user);
                _userRepository.UnitOfWork.Commit();
                userId = user.Id;
            }
            else
            {
                userId = linUserIdentity.CreateUserId;
                await _userRepository.UpdateLastLoginTimeAsync(linUserIdentity.CreateUserId);
            }

            return userId;

        }

        /// <summary>
        /// qq快速登录的信息，唯一值openid,昵称(nickname)，性别(gender)，picture（30像素）,picture_medium(50像素），picture_full 100 像素，avatar（40像素），avatar_full(100像素）
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<long> SaveQQAsync(ClaimsPrincipal principal, string openId)
        {
            string nickname = principal.FindFirst(ClaimTypes.Name)?.Value;
            string gender = principal.FindFirst(ClaimTypes.Gender)?.Value;
            string picture = principal.FindFirst(QQAuthenticationConstants.Claims.PictureUrl)?.Value;
            string picture_medium = principal.FindFirst(QQAuthenticationConstants.Claims.PictureMediumUrl)?.Value;
            string picture_full = principal.FindFirst(QQAuthenticationConstants.Claims.PictureFullUrl)?.Value;
            string avatar = principal.FindFirst(QQAuthenticationConstants.Claims.AvatarUrl)?.Value;
            string avatar_full = principal.FindFirst(QQAuthenticationConstants.Claims.AvatarFullUrl)?.Value;
            
            Expression<Func<LinUserIdentity, bool>> expression = r => 
                r.IdentityType == LinUserIdentity.QQ&& r.Credential == openId;

            LinUserIdentity linUserIdentity =await _freeSql.Select<LinUserIdentity>().Where(expression).FirstAsync();

            long userId = 0;
            if (linUserIdentity == null)
            {
                LinUser user = new LinUser
                {
                    Admin = (int)UserAdmin.Common,
                    Active = (int)UserActive.Active,
                    Avatar = avatar_full,
                    CreateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    Email = "",
                    Introduction =  "",
                    LinUserGroups = new List<LinUserGroup>()
                    {
                        new LinUserGroup()
                        {
                            GroupId = LinConsts.Group.User
                        }
                    },
                    Nickname = nickname,
                    Username = "",
                    BlogAddress = "",
                    LinUserIdentitys = new List<LinUserIdentity>()
                    {
                        new LinUserIdentity
                        {
                            CreateTime = DateTime.Now,
                            Credential = openId,
                            IdentityType = LinUserIdentity.GitHub,
                            Identifier = nickname,
                        }
                    }
                };
                await _userRepository.InsertAsync(user);
                _userRepository.UnitOfWork.Commit();
                userId = user.Id;
            }
            else
            {
                userId = linUserIdentity.CreateUserId;
                await _userRepository.UpdateLastLoginTimeAsync(linUserIdentity.CreateUserId);
            }

            return userId;
        }

        public bool VerifyUsernamePassword(long userId, string username, string password)
        {
            LinUserIdentity userIdentity = _freeSql.Select<LinUserIdentity>()
                .Where(r => r.CreateUserId == userId && r.Identifier == username)
                .ToOne();

            return userIdentity != null && EncryptUtil.Verify(userIdentity.Credential, password);
        }

        public async Task ChangePasswordAsync(long userId, string newpassword)
        {
            string encryptPassword = EncryptUtil.Encrypt(newpassword);

            await _freeSql.Update<LinUserIdentity>()
                .Where(r=>r.CreateUserId==userId&&r.IdentityType==LinUserIdentity.Password)
                .Set(a => new LinUserIdentity()
                {
                    Credential = encryptPassword
                }).ExecuteAffrowsAsync();
        }

        public async Task DeleteAsync(long userId)
        {
            await _freeSql.Select<LinUserIdentity>().Where(r => r.CreateUserId == userId).ToDelete().ExecuteAffrowsAsync();
        }

    }
}
