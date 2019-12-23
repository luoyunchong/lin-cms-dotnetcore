using AspNet.Security.OAuth.GitHub;
using LinCms.Web.Services.Cms.Interfaces;
using LinCms.Zero.Common;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LinCms.Web.Services.Cms
{
    public class UserCommunityService : IUserCommunityService
    {
        public UserCommunityService(IFreeSql freeSql)
        {
            FreeSql = freeSql;
        }

        public IFreeSql FreeSql { get; }

        public long SaveGitHub(ClaimsPrincipal principal, string openId)
        {
            //TODO 记录授权成功后的信息 
            string email = principal.FindFirst(ClaimTypes.Email)?.Value;
            string name = principal.FindFirst(ClaimTypes.Name)?.Value;
            string gitHubName = principal.FindFirst(GitHubAuthenticationConstants.Claims.Name)?.Value;
            string gitHubApiUrl = principal.FindFirst(GitHubAuthenticationConstants.Claims.Url)?.Value;
            string avatarUrl = principal.FindFirst(LinConsts.Claims.AvatarUrl)?.Value;
            string bio = principal.FindFirst(LinConsts.Claims.BIO)?.Value;
            string blogAddress = principal.FindFirst(LinConsts.Claims.BlogAddress)?.Value;
            Expression<Func<LinUserCommunity, bool>> expression = r => r.IdentityType == LinUserCommunity.GitHub && r.OpenId == openId;

            LinUserCommunity linUserCommunity = FreeSql.Select<LinUserCommunity>().Where(expression).First();

            long userId = 0;
            if (linUserCommunity == null)
            {
                FreeSql.Transaction(() =>
                {
                    userId = FreeSql.Insert(new LinUser
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

                    FreeSql.Insert(new LinUserCommunity
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

                FreeSql.Update<LinUserCommunity>(linUserCommunity.Id).Set(r => new LinUserCommunity()
                {
                    BlogAddress = blogAddress,
                }).ExecuteAffrows();
            }

            return userId;

        }
    }
}
