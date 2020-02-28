// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using DotNetCore.CAP.Transport;
using FreeSql;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore.CAP
{
    public class MySqlCapTransaction : CapTransactionBase
    {
        public MySqlCapTransaction(
            IDispatcher dispatcher) : base(dispatcher)
        {
        }

        public override void Commit()
        {
            Debug.Assert(DbTransaction != null);

            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Commit();
                    break;
                case IUnitOfWork unitOfWork:
                    unitOfWork.Commit();
                    break;
            }
            Flush();
        }

        public override Task CommitAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(DbTransaction != null);

            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Commit();
                    break;
                case IUnitOfWork unitOfWork:
                    unitOfWork.Commit();
                    break;
            }
            Flush();
            return Task.CompletedTask;
        }

        public override void Rollback()
        {
            Debug.Assert(DbTransaction != null);

            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Rollback();
                    break;
                case IUnitOfWork unitOfWork:
                    unitOfWork.Rollback();
                    break;
            }
        }

        public override Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            Debug.Assert(DbTransaction != null);

            switch (DbTransaction)
            {
                case IDbTransaction dbTransaction:
                    dbTransaction.Rollback();
                    break;
                case IUnitOfWork unitOfWork:
                    unitOfWork.Rollback();
                    break;
            }

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            (DbTransaction as IDbTransaction)?.Dispose();
            DbTransaction = null;
        }
    }

    public static class CapTransactionExtensions
    {

        public static ICapTransaction Begin(this ICapTransaction transaction,
            IDbTransaction dbTransaction, bool autoCommit = false)
        {
            transaction.DbTransaction = dbTransaction;
            transaction.AutoCommit = autoCommit;

            return transaction;
        }

        public static ICapTransaction Begin(this ICapTransaction transaction,
            IUnitOfWork unitOfWork, bool autoCommit = false)
        {
            transaction.DbTransaction = unitOfWork;
            transaction.AutoCommit = autoCommit;

            return transaction;
        }

        /// <summary>
        /// Start the CAP transaction
        /// </summary>
        /// <param name="dbConnection">The <see cref="IDbConnection" />.</param>
        /// <param name="publisher">The <see cref="ICapPublisher" />.</param>
        /// <param name="autoCommit">Whether the transaction is automatically committed when the message is published</param>
        /// <returns>The <see cref="ICapTransaction" /> object.</returns>
        public static ICapTransaction BeginTransaction(this IDbConnection dbConnection,
            ICapPublisher publisher, bool autoCommit = false)
        {
            if (dbConnection.State == ConnectionState.Closed)
            {
                dbConnection.Open();
            }

            var dbTransaction = dbConnection.BeginTransaction();
            publisher.Transaction.Value = publisher.ServiceProvider.GetService<ICapTransaction>();
            return publisher.Transaction.Value.Begin(dbTransaction, autoCommit);
        }

        public static ICapTransaction BeginTransaction(this IUnitOfWork unitOfWork,
            ICapPublisher publisher, bool autoCommit = false)
        {
            publisher.Transaction.Value = publisher.ServiceProvider.GetService<ICapTransaction>();
            return publisher.Transaction.Value.Begin(unitOfWork, autoCommit);
        }
    }
}