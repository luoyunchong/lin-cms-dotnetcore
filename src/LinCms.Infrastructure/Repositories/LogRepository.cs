using System;
using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Repositories;

public class LogRepository : AuditDefaultRepository<LinLog, Guid, long>, ILogRepository
{
    public LogRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser)
        : base(unitOfWorkManager, currentUser)
    {

    }

    public void Create(LinLog linlog)
    {
        linlog.CreateTime = DateTime.Now;
        linlog.Username = CurrentUser.UserName;
        linlog.UserId = CurrentUser.FindUserId();

        base.Insert(linlog);
    }
}