using AspNet.Security.OAuth.GitHub;
using AspNet.Security.OAuth.QQ;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Gitee;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using AutoMapper;
using LinCms.Core.Data;
using LinCms.Core.Security;
using LinCms.Core.Exceptions;
using System.Linq;

namespace LinCms.Application.Cms.Users
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditBaseRepository<LinUserIdentity> _userIdentityRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public UserIdentityService(IAuditBaseRepository<LinUserIdentity> userIdentityRepository,
            IUserRepository userRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _userIdentityRepository = userIdentityRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            this._currentUser = currentUser;
        }

        /// <summary>
        /// 记录授权成功后的信息
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<long> SaveGitHubAsync(ClaimsPrincipal principal, string openId)
        {
            LinUserIdentity linUserIdentity = await _userIdentityRepository.Where(r => r.IdentityType == LinUserIdentity.GitHub && r.Credential == openId).FirstAsync();

            long userId = 0;
            if (linUserIdentity == null)
            {
                string email = principal.FindFirst(ClaimTypes.Email)?.Value;
                string name = principal.FindFirst(ClaimTypes.Name)?.Value;
                string gitHubName = principal.FindFirst(GitHubAuthenticationConstants.Claims.Name)?.Value;
                string gitHubApiUrl = principal.FindFirst(GitHubAuthenticationConstants.Claims.Url)?.Value;
                string HtmlUrl = principal.FindFirst(LinConsts.Claims.HtmlUrl)?.Value;
                string avatarUrl = principal.FindFirst(LinConsts.Claims.AvatarUrl)?.Value;
                string bio = principal.FindFirst(LinConsts.Claims.BIO)?.Value;
                string blogAddress = principal.FindFirst(LinConsts.Claims.BlogAddress)?.Value;

                LinUser user = new LinUser
                {
                    Active = UserActive.Active,
                    Avatar = avatarUrl,
                    CreateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    Email = email,
                    Introduction = bio + HtmlUrl,
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
                        new LinUserIdentity(LinUserIdentity.GitHub,name,openId,DateTime.Now)
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
        public async Task<long> SaveQQAsync(ClaimsPrincipal principal, string openId)
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

                LinUser user = new LinUser
                {
                    Active = UserActive.Active,
                    Avatar = avatarFullUrl,
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
                        new LinUserIdentity(LinUserIdentity.QQ, nickname, openId,DateTime.Now)
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
        public async Task<long> SaveGiteeAsync(ClaimsPrincipal principal, string openId)
        {

            LinUserIdentity linUserIdentity = await _userIdentityRepository.Where(r => r.IdentityType == LinUserIdentity.Gitee && r.Credential == openId).FirstAsync();

            long userId = 0;
            if (linUserIdentity == null)
            {

                string email = principal.FindFirst(ClaimTypes.Email)?.Value;
                string name = principal.FindFirst(ClaimTypes.Name)?.Value;

                //string giteeUrl = principal.FindFirst(GiteeAuthenticationConstants.Claims.Url)?.Value;
                string nickname = principal.FindFirst(GiteeAuthenticationConstants.Claims.Name)?.Value;

                string avatarUrl = principal.FindFirst("urn:gitee:avatar_url")?.Value;
                string blogAddress = principal.FindFirst("urn:gitee:blog")?.Value;
                string bio = principal.FindFirst("urn:gitee:bio")?.Value;
                string htmlUrl = principal.FindFirst("urn:gitee:html_url")?.Value;

                LinUser user = new LinUser
                {
                    Active = UserActive.Active,
                    Avatar = avatarUrl,
                    LastLoginTime = DateTime.Now,
                    Email = email,
                    Introduction = bio + htmlUrl,
                    LinUserGroups = new List<LinUserGroup>()
                    {
                        new LinUserGroup()
                        {
                            GroupId = LinConsts.Group.User
                        }
                    },
                    Nickname = nickname,
                    Username = "",
                    BlogAddress = blogAddress,
                    LinUserIdentitys = new List<LinUserIdentity>()
                    {
                        new LinUserIdentity(LinUserIdentity.Gitee,name,openId,DateTime.Now)
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
            var linUserIdentity = await _userIdentityRepository.Where(a => a.CreateUserId == userId && a.IdentityType == LinUserIdentity.Password).FirstAsync();
            await this.ChangePasswordAsync(linUserIdentity, newpassword);
        }

        public async Task ChangePasswordAsync(LinUserIdentity linUserIdentity, string newpassword)
        {
            string encryptPassword = EncryptUtil.Encrypt(newpassword);
            if (linUserIdentity == null)
            {
                linUserIdentity = new LinUserIdentity(LinUserIdentity.Password, "", encryptPassword, DateTime.Now);
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

        public async Task<List<UserIdentityDto>> GetListAsync(long userId)
        {
            List<LinUserIdentity> userIdentities = await _userIdentityRepository
                .Where(r => r.CreateUserId == userId)
                .ToListAsync();

            return _mapper.Map<List<UserIdentityDto>>(userIdentities);
        }

        public async Task<UnifyResponseDto> BindGitHubAsync(ClaimsPrincipal principal, string openId, long userId)
        {
            string name = principal.FindFirst(ClaimTypes.Name)?.Value;
            return await this.BindAsync(LinUserIdentity.GitHub, name, openId, userId);
        }

        public async Task<UnifyResponseDto> BindQQAsync(ClaimsPrincipal principal, string openId, long userId)
        {
            string nickname = principal.FindFirst(ClaimTypes.Name)?.Value;
            return await this.BindAsync(LinUserIdentity.QQ, nickname, openId, userId);
        }

        public async Task<UnifyResponseDto> BindGiteeAsync(ClaimsPrincipal principal, string openId, long userId)
        {
            string name = principal.FindFirst(ClaimTypes.Name)?.Value;
            return await this.BindAsync(LinUserIdentity.Gitee, name, openId, userId);
        }

        private async Task<UnifyResponseDto> BindAsync(string identityType, string name, string openId, long userId)
        {
            LinUserIdentity linUserIdentity = await _userIdentityRepository.Where(r => r.IdentityType == identityType && r.Credential == openId).FirstAsync();
            if (linUserIdentity == null)
            {
                var userIdentity = new LinUserIdentity(identityType, name, openId, DateTime.Now);
                userIdentity.CreateUserId = userId;
                await _userIdentityRepository.InsertAsync(userIdentity);
                return UnifyResponseDto.Success("绑定成功");
            }
            else
            {
                return UnifyResponseDto.Error("绑定失败,该用户已绑定其他账号");
            }
        }

        public async Task UnBind(Guid id)
        {
            LinUserIdentity userIdentity = await _userIdentityRepository.GetAsync(id);
            if (userIdentity == null || userIdentity.CreateUserId != _currentUser.Id)
            {
                throw new LinCmsException("你无权解绑此账号");
            }

            List<LinUserIdentity> userIdentities = await _userIdentityRepository.Select.Where(r => r.CreateUserId == _currentUser.Id).ToListAsync();

            bool hasPwd = userIdentities.Where(r => r.IdentityType == LinUserIdentity.Password).Any();

            if (!hasPwd && userIdentities.Count == 1)
            {
                throw new LinCmsException("你未设置密码，无法解绑最后一个第三方登录账号");
            }
            await _userIdentityRepository.DeleteAsync(userIdentity);
        }
    }
}