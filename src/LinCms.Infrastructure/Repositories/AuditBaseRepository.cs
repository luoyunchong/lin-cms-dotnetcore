using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Infrastructure.Repositories
{
    public class AuditBaseRepository<TEntity> : AuditBaseRepository<TEntity, Guid>, IAuditBaseRepository<TEntity> where TEntity : class, new()
    {
        public AuditBaseRepository(IUnitOfWork uow, ICurrentUser currentUser, IFreeSql fsql, Expression<Func<TEntity, bool>> filter = null, Func<string, string> asTable = null)
            : base(uow, currentUser, fsql, filter, asTable)
        {
        }
    }
    /// <summary>
    /// 审计仓储：实现如果实体类
    /// 继承了ICreateAduitEntity  则自动增加创建时间/人信息
    /// 继承了IUpdateAuditEntity，更新时，修改更新时间/人
    /// 继承了ISoftDeleteAduitEntity，删除时，自动改成软删除。仅注入此仓储或继承此仓储的实现才能实现如上功能。
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class AuditBaseRepository<TEntity, TKey> : BaseRepository<TEntity, TKey>, IAuditBaseRepository<TEntity, TKey>
        where TEntity : class, new()
    {
        protected readonly ICurrentUser CurrentUser;
        public AuditBaseRepository(IUnitOfWork uow, ICurrentUser currentUser, IFreeSql fsql, Expression<Func<TEntity, bool>> filter = null, Func<string, string> asTable = null) : base(fsql, filter, asTable)
        {
            CurrentUser = currentUser;
            base.UnitOfWork = uow;
        }

        private void BeforeInsert(TEntity entity)
        {
            if (!(entity is ICreateAduitEntity e)) return;
            e.CreateTime = DateTime.Now;

            e.CreateUserId = CurrentUser.Id ?? 0;

            if (!(entity is IUpdateAuditEntity updateAuditEntity)) return;
            updateAuditEntity.UpdateTime = DateTime.Now;
            updateAuditEntity.UpdateUserId = CurrentUser.Id; ;
        }

        public override TEntity Insert(TEntity entity)
        {
            BeforeInsert(entity);
            return base.Insert(entity);
        }

        public override Task<TEntity> InsertAsync(TEntity entity)
        {
            this.BeforeInsert(entity);
            return base.InsertAsync(entity);
        }

        public override List<TEntity> Insert(IEnumerable<TEntity> entitys)
        {
            foreach (TEntity entity in entitys)
            {
                BeforeInsert(entity);
            }

            return base.Insert(entitys);
        }

        public override Task<List<TEntity>> InsertAsync(IEnumerable<TEntity> entitys)
        {
            foreach (TEntity entity in entitys)
            {
                BeforeInsert(entity);
            }
            return base.InsertAsync(entitys);
        }

        private void BeforeUpdate(TEntity entity)
        {
            if (!(entity is IUpdateAuditEntity e)) return;
            e.UpdateTime = DateTime.Now;
            e.UpdateUserId = CurrentUser.Id;
        }

        public new int Update(TEntity entity)
        {
            BeforeUpdate(entity);
            return base.Update(entity);
        }

        public new Task<int> UpdateAsync(TEntity entity)
        {
            BeforeUpdate(entity);
            return base.UpdateAsync(entity);
        }

        public new int Update(IEnumerable<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                this.BeforeUpdate(entity);
            }
            return base.Update(entitys);
        }

        public new Task<int> UpdateAsync(IEnumerable<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                BeforeUpdate(entity);
            }
            return base.UpdateAsync(entitys);
        }

        public new int Delete(TEntity entity)
        {
            if (entity is IDeleteAduitEntity)
            {
                return Orm.Update<TEntity>(entity)
                           .Set(a => (a as IDeleteAduitEntity).IsDeleted, true)
                           .Set(a => (a as IDeleteAduitEntity).DeleteUserId, CurrentUser.Id)
                           .Set(a => (a as IDeleteAduitEntity).DeleteTime, DateTime.Now)
                           .ExecuteAffrows();
            }

            return base.Delete(entity);
        }

        public new int Delete(IEnumerable<TEntity> entitys)
        {
            if (entitys.Any())
            {
                Attach(entitys);
                foreach (TEntity x1 in entitys)
                {
                    if (x1 is IDeleteAduitEntity softDelete)
                    {
                        softDelete.DeleteUserId = CurrentUser.Id;
                        softDelete.DeleteTime = DateTime.Now;
                        softDelete.IsDeleted = true;
                    }
                }

                return Update(entitys);
            }

            return base.Delete(entitys);
        }


        public new Task<int> DeleteAsync(IEnumerable<TEntity> entitys)
        {
            if (entitys.Any())
            {
                Attach(entitys);
                foreach (TEntity x1 in entitys)
                {
                    if (x1 is IDeleteAduitEntity softDelete)
                    {
                        softDelete.DeleteUserId = CurrentUser.Id;
                        softDelete.DeleteTime = DateTime.Now;
                        softDelete.IsDeleted = true;
                    }
                }
                return UpdateAsync(entitys);
            }
            return base.DeleteAsync(entitys);
        }

        public new async Task<int> DeleteAsync(TEntity entity)
        {
            if (entity is IDeleteAduitEntity)
            {
                return await Orm.Update<TEntity>(entity)
                    .Set(a => (a as IDeleteAduitEntity).IsDeleted, true)
                    .Set(a => (a as IDeleteAduitEntity).DeleteUserId, CurrentUser.Id)
                    .Set(a => (a as IDeleteAduitEntity).DeleteTime, DateTime.Now)
                    .ExecuteAffrowsAsync();
            }

            return base.Delete(entity);
        }
        public new int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(IDeleteAduitEntity).IsAssignableFrom(typeof(TEntity)))
            {
                List<TEntity> items = Orm.Select<TEntity>().Where(predicate).ToList();
                return Orm.Update<TEntity>(items)
                    .Set(a => (a as IDeleteAduitEntity).IsDeleted, true)
                    .Set(a => (a as IDeleteAduitEntity).DeleteUserId, CurrentUser.Id)
                    .Set(a => (a as IDeleteAduitEntity).DeleteTime, DateTime.Now)
                    .ExecuteAffrows();
            }

            return base.Delete(predicate);
        }

        public new async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(IDeleteAduitEntity).IsAssignableFrom(typeof(TEntity)))
            {
                List<TEntity> items = Orm.Select<TEntity>().Where(predicate).ToList();
                return await Orm.Update<TEntity>(items)
                     .Set(a => (a as IDeleteAduitEntity).IsDeleted, true)
                     .Set(a => (a as IDeleteAduitEntity).DeleteUserId, CurrentUser.Id)
                     .Set(a => (a as IDeleteAduitEntity).DeleteTime, DateTime.Now)
                     .ExecuteAffrowsAsync();
            }

            return await base.DeleteAsync(predicate);
        }
        public new async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            BeforeInsert(entity);
            BeforeUpdate(entity);
            await base.InsertOrUpdateAsync(entity);
            return entity;
        }

    }
}
