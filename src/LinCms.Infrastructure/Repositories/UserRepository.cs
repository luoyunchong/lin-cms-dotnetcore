using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Core.Entities;
using LinCms.Core.Security;

namespace LinCms.Infrastructure.Repositories
{
    public class UserRepository : AuditBaseRepository<LinUser>
    {
        public UserRepository(IUnitOfWork unitOfWork, ICurrentUser currentUser, IFreeSql fsql, Expression<Func<LinUser, bool>> filter = null, Func<string, string> asTable = null)
            : base(unitOfWork, currentUser, fsql, filter, asTable)
        {
        }

        public Task UpdateLastLoginTimeAsync(long id)
        {
            return UpdateDiy.Set(r => new LinUser()
            {
                LastLoginTime = DateTime.Now
            }).Where(r => r.Id == id).ExecuteAffrowsAsync();
        }
    }
}
