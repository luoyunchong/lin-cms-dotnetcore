using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using JetBrains.Annotations;

namespace LinCms.Web.Uow
{
    public interface IUnitOfWorkManager
    {
        [CanBeNull]
        IUnitOfWork Current { get; }
        [NotNull]
        IUnitOfWork Begin(bool requiresNew = false);

    }
}
