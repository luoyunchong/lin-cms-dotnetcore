using System;
using System.Linq.Expressions;
using System.Security.Claims;
using AspNet.Security.OAuth.GitHub;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;

namespace LinCms.Application.Cms.Users
{
    public class UserCommunityService : IUserCommunityService
    {
        private readonly IFreeSql _freeSql;

        public UserCommunityService(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        /// <summary>
        /// 记录授权成功后的信息
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public long SaveGitHub(ClaimsPrincipal principal, string openId)
        {
            string email = principal.FindFirst(ClaimTypes.Email)?.Value;
            string name = principal.FindFirst(ClaimTypes.Name)?.Value;
            string gitHubName = principal.FindFirst(GitHubAuthenticationConstants.Claims.Name)?.Value;
            string gitHubApiUrl = principal.FindFirst(GitHubAuthenticationConstants.Claims.Url)?.Value;
            string avatarUrl = principal.FindFirst(LinConsts.Claims.AvatarUrl)?.Value;
            string bio = principal.FindFirst(LinConsts.Claims.BIO)?.Value;
            string blogAddress = principal.FindFirst(LinConsts.Claims.BlogAddress)?.Value;
            Expression<Func<LinUserCommunity, bool>> expression = r => r.IdentityType == LinUserCommunity.GitHub && r.OpenId == openId;

            LinUserCommunity linUserCommunity = _freeSql.Select<LinUserCommunity>().Where(expression).First();

            long userId = 0;
            if (linUserCommunity == null)
            {
                _freeSql.Transaction(() =>
                {
                    userId = _freeSql.Insert(new LinUser
                    {
                        Admin = (int)UserAdmin.Common,
                        Active = (int)UserActive.Active,
                        Avatar = avatarUrl,
                        CreateTime = DateTime.Now,
                        Email = email,
                        Introduction = bio,
                        GroupId = LinConsts.Group.User,
                        Nickname = gitHubName,
                        Username = email
                    }).ExecuteIdentity();

                    _freeSql.Insert(new LinUserCommunity
                    {
                        CreateTime = DateTime.Now,
                        OpenId = openId,
                        IdentityType = LinUserCommunity.GitHub,
                        UserName = name,
                        BlogAddress = blogAddress,
                        CreateUserId = (long)userId
                    }).ExecuteAffrows();
                });
            }
            else
            {
                userId = linUserCommunity.CreateUserId;

                _freeSql.Update<LinUserCommunity>(linUserCommunity.Id).Set(r => new LinUserCommunity()
                {
                    BlogAddress = blogAddress,
                }).ExecuteAffrows();
            }

            return userId;

        }
    }
}
