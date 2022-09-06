using System.Reflection;
using DotNetCore.CAP;
using FreeSql;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms;

public static class CapUnitOfWorkExtensions
{

    public static void Flush(this ICapTransaction capTransaction)
    {
        capTransaction?.GetType().GetMethod("Flush", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(capTransaction, null);
    }


    public static ICapTransaction BeginTransaction(this IUnitOfWork unitOfWork, ICapPublisher publisher, bool autoCommit = false)
    {
        //看了源码，换了新的写法，换不同的数据库，就需要手动修改这段代码了（MySqlCapTransaction）
        //publisher.Transaction.Value = (ICapTransaction)publisher.ServiceProvider.GetService(typeof(ICapTransaction));新版本只能得到null
        publisher.Transaction.Value = ActivatorUtilities.CreateInstance<MySqlCapTransaction>(publisher.ServiceProvider);
        return publisher.Transaction.Value.Begin(unitOfWork.GetOrBeginTransaction(), autoCommit);
    }

    /// <summary>
    /// 提交CAP 和FreeSql的UnitOfWork事务
    /// </summary>
    /// <param name="capTransaction"></param>
    /// <param name="unitOfWork"></param>
    public static void Commit(this ICapTransaction capTransaction, IUnitOfWork unitOfWork)
    {
        unitOfWork.Commit();
        capTransaction.Flush();
    }


    //public static ICapTransaction BeginTransaction(this UnitOfWorkManager unitOfWorkManager, ICapPublisher publisher, bool autoCommit = false)
    //{
    //    publisher.Transaction.Value = ActivatorUtilities.CreateInstance<MySqlCapTransaction>(publisher.ServiceProvider);
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