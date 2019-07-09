using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LinCms.Web.Domain;
using LinCms.Web.Helpers;
using LinCms.Web.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LinCms.Web.Services
{
    public class UserService : IUserService
    {
        private List<LinUser> _linUsers = new List<LinUser>
        {
            new LinUser("super","123qwe")
        };

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string Authenticate(string nickname, string password)
        {
            var user = _linUsers.SingleOrDefault(x => x.Nickname == nickname && x.Password == password);

            // return null if LinUser not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IEnumerable<LinUser> GetAll()
        {
            // return LinUsers without passwords
            return _linUsers.Select(x =>
            {
                x.Password = null;
                return x;
            });
        }
    }
}
