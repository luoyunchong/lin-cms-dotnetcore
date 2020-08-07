using AspNet.Security.OAuth.GitHub;
using LinCms.Common;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.IRepositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LinCms.Cms.Users
{
    public class GithubOAuth2Serivice : OAuthService, IOAuth2Service
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditBaseRepository<LinUserIdentity> _userIdentityRepository;

        public GithubOAuth2Serivice(IAuditBaseRepository<LinUserIdentity> userIdentityRepository, IUserRepository userRepository) : base(userIdentityRepository)
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
        public override async Task<long> SaveUserAsync(ClaimsPrincipal principal, string openId)
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
            }

            return userId;
        }
    }
}
