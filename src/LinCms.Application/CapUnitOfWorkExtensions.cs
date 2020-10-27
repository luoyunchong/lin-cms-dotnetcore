using DotNetCore.CAP;
using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LinCms
{
    public static class CapUnitOfWorkExtensions
    {
        public static ICapTransaction BeginTransaction(this IUnitOfWork unitOfWork, ICapPublisher publisher, bool autoCommit = false)
        {
            publisher.Transaction.Value = (ICapTransaction)publisher.ServiceProvider.GetService(typeof(ICapTransaction));
            return publisher.Transaction.Value.Begin(unitOfWork.GetOrBeginTransaction(), autoCommit);
        }

        public static void Flush(this ICapTransaction capTransaction)
        {
            capTransaction?.GetType().GetMethod("Flush", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(capTransaction, null);
        }

        public static void Commit(this IUnitOfWork unitOfWork, ICapTransaction capTransaction)
        {
            unitOfWork.Commit();
            capTransaction.Flush();
        }
    }

}
