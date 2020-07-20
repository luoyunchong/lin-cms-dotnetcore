using System;
using FreeSql;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Repositories
{
    public class LogRepository : AuditBaseRepository<LinLog>, ILogRepository
    {
        public LogRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser)
            : base(unitOfWorkManager, currentUser)
        {

        }

        public void Create(LinLog linlog)
        {
            linlog.CreateTime = DateTime.Now;
            linlog.Username = CurrentUser.UserName;
            linlog.UserId = CurrentUser.Id ?? 0;

            base.Insert(linlog);
        }
    }
}
