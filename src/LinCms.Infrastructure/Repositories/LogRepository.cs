using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Infrastructure.Repositories
{
    public class LogRepository : AuditBaseRepository<LinLog>, ILogRepository
    {
        public LogRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser)
            : base(unitOfWorkManager, currentUser)
        {

        }

        public void Create(LinLog linlog)
        {
            linlog.Time = DateTime.Now;
            linlog.UserName = CurrentUser.UserName;
            linlog.UserId = CurrentUser.Id ?? 0;

            base.Insert(linlog);
        }
    }
}
