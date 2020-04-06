using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using FreeSql;

namespace LinCms.Web.Uow
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnitOfWorkInterceptor(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }
        public void Intercept(IInvocation invocation)
        {
            using (_unitOfWork)
            {
                _unitOfWork.Open();
                invocation.Proceed();
                _unitOfWork.Commit();
            }
        }
    }
}
