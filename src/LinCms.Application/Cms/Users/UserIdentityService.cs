using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using AspNet.Security.OAuth.QQ;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Cms.Users
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditBaseRepository<LinUserIdentity> _userIdentityRepository;

        public UserIdentityService(IAuditBaseRepository<LinUserIdentity> userIdentityRepository,
            IUserRepository userRepository)
        {
            _userIdentityRepository = userIdentityRepository;
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
            Expression<Func<LinUserIdentity, bool>> expression = r =>
                r.IdentityType == LinUserIdentity.GitHub && r.Credential == openId;

            LinUserIdentity linUserIdentity = await _userIdentityRepository.Where(expression).FirstAsync();

            long userId = 0;
            if (linUserIdentity == null)
            {
                LinUser user = new LinUser
                {
                    Active = (int) UserActive.Active,
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
            string avatarUrl = principal.FindFirst(QQAuthenticationConstants.Claims.AvatarUrl)?.Value;
            string avatarFullUrl = principal.FindFirst(QQAuthenticationConstants.Claims.AvatarFullUrl)?.Value;

            Expression<Func<LinUserIdentity, bool>> expression = r =>
                r.IdentityType == LinUserIdentity.QQ && r.Credential == openId;

            LinUserIdentity linUserIdentity = await _userIdentityRepository.Where(expression).FirstAsync();

            long userId = 0;
            if (linUserIdentity == null)
            {
                LinUser user = new LinUser
                {
                    Active = (int) UserActive.Active,
                    Avatar = avatarFullUrl,
                    CreateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    Email = "",
                    Introduction = "",
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
                            IdentityType = LinUserIdentity.QQ,
                            Identifier = nickname,
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

        public async Task<bool> VerifyUserPasswordAsync(long userId, string password)
        {
            LinUserIdentity userIdentity = await this.GetFirstByUserIdAsync(userId);

            return userIdentity != null && EncryptUtil.Verify(userIdentity.Credential, password);
        }

        public async Task ChangePasswordAsync(long userId, string newpassword)
        {
            var linUserIdentity = await _userIdentityRepository.Where(a => a.CreateUserId == userId&& a.IdentityType==LinUserIdentity.Password).FirstAsync();
            await this.ChangePasswordAsync(linUserIdentity, newpassword);
        }
        
        public async Task ChangePasswordAsync(LinUserIdentity linUserIdentity,string newpassword)
        {
            string encryptPassword = EncryptUtil.Encrypt(newpassword);
            if (linUserIdentity == null)
            {
                linUserIdentity=new LinUserIdentity()
                {
                    IdentityType = LinUserIdentity.Password,
                    Identifier = "",
                    Credential = encryptPassword
                };
                await _userIdentityRepository.InsertAsync(linUserIdentity);
            }
            else
            {
                linUserIdentity.Credential = encryptPassword;
                await _userIdentityRepository.UpdateAsync(linUserIdentity);
            }
      
        }

        public async Task DeleteAsync(long userId)
        {
            await _userIdentityRepository.Where(r => r.CreateUserId == userId).ToDelete().ExecuteAffrowsAsync();
        }

        public async Task<LinUserIdentity> GetFirstByUserIdAsync(long userId)
        {
            return await _userIdentityRepository
                .Where(r => r.CreateUserId == userId && r.IdentityType == LinUserIdentity.Password)
                .ToOneAsync();
        }
    }
}