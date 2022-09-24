using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCore.Security;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Security;
using Microsoft.Extensions.Logging;

namespace LinCms.Domain;

public class TokenManager : ITokenManager, ITransientDependency
{
    private readonly ILogger<TokenManager> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jsonWebTokenService;
    public TokenManager(IJwtService jsonWebTokenService, ILogger<TokenManager> logger, IUserRepository userRepository)
    {
        _jsonWebTokenService = jsonWebTokenService;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<Tokens> CreateTokenAsync(LinUser user)
    {
        List<Claim> claims = new()
        {
            new Claim (FreeKitClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim (FreeKitClaimTypes.Email, user.Email?? ""),
            new Claim (FreeKitClaimTypes.Name, user.Nickname?? ""),
            new Claim (FreeKitClaimTypes.UserName, user.Username?? ""),
        };
        user.LinGroups?.ForEach(r =>
        {
            claims.Add(new Claim(FreeKitClaimTypes.Role, r.Name));
            claims.Add(new Claim(LinCmsClaimTypes.GroupIds, r.Id.ToString()));
        });

        string token = _jsonWebTokenService.Encode(claims);

        user.AddRefreshToken();
        await _userRepository.UpdateAsync(user);

        return new Tokens(token, user.RefreshToken);
    }
}