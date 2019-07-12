using LinCms.Web.Domain;
using LinCms.Web.Services.Interfaces;

namespace LinCms.Web.Services
{
    public class UserService : IUserSevice
    {
        private readonly IFreeSql _freeSql;

        public UserService(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        public LinUser Authorization(string username, string password)
        {
            LinUser user = _freeSql.Select<LinUser>().Where(r => r.Nickname == username && r.Password == password).First();

            return user;
        }
    }
}
