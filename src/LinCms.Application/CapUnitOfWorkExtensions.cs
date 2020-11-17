using DotNetCore.CAP;
using FreeSql;
using System.Reflection;

namespace LinCms
{
    public static class CapUnitOfWorkExtensions
    {

        public static void Flush(this ICapTransaction capTransaction)
        {
            capTransaction?.GetType().GetMethod("Flush", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(capTransaction, null);
        }

       
        public static ICapTransaction BeginTransaction(this IUnitOfWork unitOfWork, ICapPublisher publisher, bool autoCommit = false)
        {
            publisher.Transaction.Value = (ICapTransaction)publisher.ServiceProvider.GetService(typeof(ICapTransaction));
            return publisher.Transaction.Value.Begin(unitOfWork.GetOrBeginTransaction(), autoCommit);
        }

        public static void Commit(this ICapTransaction capTransaction, IUnitOfWork unitOfWork)
        {
            unitOfWork.Commit();
            capTransaction.Flush();
        }


        //public static ICapTransaction BeginTransaction(this UnitOfWorkManager unitOfWorkManager, ICapPublisher publisher, bool autoCommit = false)
        //{
        //    publisher.Transaction.Value = (ICapTransaction)publisher.ServiceProvider.GetService(typeof(ICapTransaction));

        //    IUnitOfWork unitOfWork = unitOfWorkManager.Begin(Propagation.Required);

        //    return publisher.Transaction.Value.Begin(unitOfWork.GetOrBeginTransaction(), autoCommit);
        //}

        //public static void Commit(this ICapTransaction capTransaction, UnitOfWorkManager unitOfWorkManager)
        //{
        //    IUnitOfWork unitOfWork = unitOfWorkManager.Current;
        //    if (unitOfWork != null)
        //    {
        //        unitOfWork.Commit();
        //    }
        //    capTransaction.Flush();
        //}
    }
}
