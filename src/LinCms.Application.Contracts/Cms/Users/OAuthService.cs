using System;
using System.Security.Claims;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities;
using LinCms.IRepositories;

namespace LinCms.Cms.Users
{
    public abstract class OAuthService : IOAuth2Service
    {
        private readonly IAuditBaseRepository<LinUserIdentity> _userIdentityRepository;

        public OAuthService(IAuditBaseRepository<LinUserIdentity> userIdentityRepository)
        {
            _userIdentityRepository = userIdentityRepository;
        }
        private async Task<UnifyResponseDto> BindAsync(string identityType, string name, string openId, long userId)
        {
            LinUserIdentity linUserIdentity = await _userIdentityRepository.Where(r => r.IdentityType == identityType && r.Credential == openId).FirstAsync();
            if (linUserIdentity == null)
            {
                var userIdentity = new LinUserIdentity(identityType, name, openId, DateTime.Now)
                {
                    CreateUserId = userId
                };
                await _userIdentityRepository.InsertAsync(userIdentity);
                return UnifyResponseDto.Success("绑定成功");
            }
            else
            {
                return UnifyResponseDto.Error("绑定失败,该用户已绑定其他账号");
            }
        }

        public abstract Task<long> SaveUserAsync(ClaimsPrincipal principal, string openId);

        public virtual async Task<UnifyResponseDto> BindAsync(ClaimsPrincipal principal, string identityType, string openId, long userId)
        {
            string nickname = principal.FindFirst(ClaimTypes.Name)?.Value;
            return await this.BindAsync(identityType, nickname, openId, userId);
        }

    }
}
