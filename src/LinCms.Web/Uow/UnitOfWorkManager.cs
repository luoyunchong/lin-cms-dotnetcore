using System.Threading;
using FreeSql;
using LinCms.Core.Dependency;

namespace LinCms.Web.Uow
{
    public class UnitOfWorkManager : IUnitOfWorkManager, ISingletonDependency
    {
        private readonly IFreeSql freeSql;
        private readonly IUnitOfWork unitOfWork;

        public readonly AsyncLocal<IUnitOfWork> _currentUow;
        public IUnitOfWork Current => _currentUow.Value;

        public UnitOfWorkManager(IFreeSql freeSql,IUnitOfWork unitOfWork)
        {
            this.freeSql = freeSql;
            this.unitOfWork = unitOfWork;
            _currentUow = new AsyncLocal<IUnitOfWork>();
        }


        public IUnitOfWork Begin(bool requiresNew = false)
        {
            _currentUow.Value = unitOfWork;
            if (requiresNew)
            {
                _currentUow.Value = freeSql.CreateUnitOfWork();
            }
            //_currentUow.Value.GetOrBeginTransaction();
            return _currentUow.Value;
        }

    }
}
