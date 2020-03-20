using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Infrastructure.Repositories;

namespace LinCms.Application.Cms.Users
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IFreeSql _freeSql;
        private readonly UserRepository _userRepository;
        public UserIdentityService(IFreeSql freeSql, UserRepository userRepository)
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
        public async Task<long> SaveGitHubAsync(ClaimsPrincipal principal, string openId)
        {
            string email = principal.FindFirst(ClaimTypes.Email)?.Value;
            string name = principal.FindFirst(ClaimTypes.Name)?.Value;
            string gitHubName = principal.FindFirst(GitHubAuthenticationConstants.Claims.Name)?.Value;
            string gitHubApiUrl = principal.FindFirst(GitHubAuthenticationConstants.Claims.Url)?.Value;
            string avatarUrl = principal.FindFirst(LinConsts.Claims.AvatarUrl)?.Value;
            string bio = principal.FindFirst(LinConsts.Claims.BIO)?.Value;
            string blogAddress = principal.FindFirst(LinConsts.Claims.BlogAddress)?.Value;
            Expression<Func<LinUserIdentity, bool>> expression = r => r.IdentityType == LinUserIdentity.GitHub && r.Credential == openId;

            LinUserIdentity linUserIdentity = _freeSql.Select<LinUserIdentity>().Where(expression).First();

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
                    Username = email,
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
