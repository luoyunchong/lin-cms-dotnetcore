using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Repositories
{
    public class AuditBaseRepository<TEntity> : AuditBaseRepository<TEntity, Guid>, IAuditBaseRepository<TEntity> where TEntity : class, new()
    {
        public AuditBaseRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(unitOfWorkManager, currentUser)
        {
        }
    }
    /// <summary>
    /// 审计仓储：实现如果实体类
    /// 继承了ICreateAuditEntity  则自动增加创建时间/人信息
    /// 继承了IUpdateAuditEntity，更新时，修改更新时间/人
    /// 继承了ISoftDeleteAuditEntity，删除时，自动改成软删除。仅注入此仓储或继承此仓储的实现才能实现如上功能。
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class AuditBaseRepository<TEntity, TKey> : DefaultRepository<TEntity, TKey>, IAuditBaseRepository<TEntity, TKey>
        where TEntity : class, new()
    {
        protected readonly ICurrentUser CurrentUser;
        public AuditBaseRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(unitOfWorkManager?.Orm, unitOfWorkManager)
        {
            CurrentUser = currentUser;
        }

        private void BeforeInsert(TEntity entity)
        {
            if (entity is not ICreateAuditEntity e)
            {
                return;
            }

            e.CreateTime = DateTime.Now;
            if (e.CreateUserId == 0 && CurrentUser.Id != null)
            {
                e.CreateUserId = CurrentUser.Id ?? 0;
            }

            if (entity is not IUpdateAuditEntity updateAuditEntity) return;
            updateAuditEntity.UpdateTime = DateTime.Now;
            updateAuditEntity.UpdateUserId = CurrentUser.Id;
        }

        public override TEntity Insert(TEntity entity)
        {
            BeforeInsert(entity);
            return base.Insert(entity);
        }

        public override Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            BeforeInsert(entity);
            return base.InsertAsync(entity, cancellationToken);
        }

        public override List<TEntity> Insert(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                BeforeInsert(entity);
            }

            return base.Insert(entities);
        }

        public override Task<List<TEntity>> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var enumerable = entities as TEntity[] ?? entities.ToArray();
            foreach (TEntity entity in enumerable)
            {
                BeforeInsert(entity);
            }
            return base.InsertAsync(enumerable, cancellationToken);
        }

        private void BeforeUpdate(TEntity entity)
        {
            if (entity is not IUpdateAuditEntity e) return;
            e.UpdateTime = DateTime.Now;
            e.UpdateUserId = CurrentUser.Id;
        }

        public new int Update(TEntity entity)
        {
            BeforeUpdate(entity);
            return base.Update(entity);
        }

        public new Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            BeforeUpdate(entity);
            return base.UpdateAsync(entity, cancellationToken);
        }

        public override int Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                BeforeUpdate(entity);
            }
            return base.Update(entities);
        }

        public override Task<int> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                BeforeUpdate(entity);
            }
            return base.UpdateAsync(entities, cancellationToken);
        }

        public override int Delete(TEntity entity)
        {
            if (entity is IDeleteAuditEntity)
            {
                return Orm.Update<TEntity>(entity)
                           .Set(a => (a as IDeleteAuditEntity).IsDeleted, true)
                           .Set(a => (a as IDeleteAuditEntity).DeleteUserId, CurrentUser.Id)
                           .Set(a => (a as IDeleteAuditEntity).DeleteTime, DateTime.Now)
                           .ExecuteAffrows();
            }

            return base.Delete(entity);
        }

        public override int Delete(IEnumerable<TEntity> entities)
        {
            if (entities.IsEmpty() || entities.First() is not IDeleteAuditEntity) return base.Delete(entities);

            Attach(entities);
            foreach (TEntity x1 in entities)
            {
                if (x1 is IDeleteAuditEntity softDelete)
                {
                    softDelete.DeleteUserId = CurrentUser.Id;
                    softDelete.DeleteTime = DateTime.Now;
                    softDelete.IsDeleted = true;
                }
            }
            return Update(entities);
        }

        public override async Task<int> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            TEntity entity = await GetAsync(id, cancellationToken);
            if (entity is IDeleteAuditEntity softDelete)
            {
                //softDelete.DeleteUserId = CurrentUser.Id;
                //softDelete.DeleteTime = DateTime.Now;
                //softDelete.IsDeleted = true;
                //return await base.UpdateAsync(entity, cancellationToken);
                return await Orm.Update<TEntity>(entity)
                           .Set(a => (a as IDeleteAuditEntity).IsDeleted, true)
                           .Set(a => (a as IDeleteAuditEntity).DeleteUserId, CurrentUser.Id)
                           .Set(a => (a as IDeleteAuditEntity).DeleteTime, DateTime.Now)
                           .ExecuteAffrowsAsync(cancellationToken);
            }

            return await base.DeleteAsync(id, cancellationToken);
        }


        public override Task<int> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities.IsEmpty() || entities.First() is not IDeleteAuditEntity) return base.DeleteAsync(entities, cancellationToken);

            Attach(entities);
            foreach (TEntity x1 in entities)
            {
                if (x1 is IDeleteAuditEntity softDelete)
                {
                    softDelete.DeleteUserId = CurrentUser.Id;
                    softDelete.DeleteTime = DateTime.Now;
                    softDelete.IsDeleted = true;
                }
            }
            return UpdateAsync(entities, cancellationToken);
        }

        public override async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is IDeleteAuditEntity)
            {
                return await Orm.Update<TEntity>(entity)
                    .Set(a => (a as IDeleteAuditEntity).IsDeleted, true)
                    .Set(a => (a as IDeleteAuditEntity).DeleteUserId, CurrentUser.Id)
                    .Set(a => (a as IDeleteAuditEntity).DeleteTime, DateTime.Now)
                    .ExecuteAffrowsAsync(cancellationToken);
            }

            return base.Delete(entity);
        }
        public override int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(IDeleteAuditEntity).IsAssignableFrom(typeof(TEntity)))
            {
                List<TEntity> items = Orm.Select<TEntity>().Where(predicate).ToList();
                if (items.Count == 0)
                {
                    return 0;
                }
                return Orm.Update<TEntity>(items)
                    .Set(a => (a as IDeleteAuditEntity).IsDeleted, true)
                    .Set(a => (a as IDeleteAuditEntity).DeleteUserId, CurrentUser.Id)
                    .Set(a => (a as IDeleteAuditEntity).DeleteTime, DateTime.Now)
                    .ExecuteAffrows();
            }

            return base.Delete(predicate);
        }

        public override async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (typeof(IDeleteAuditEntity).IsAssignableFrom(typeof(TEntity)))
            {
                List<TEntity> items = Orm.Select<TEntity>().Where(predicate).ToList();
                if (items.Count == 0)
                {
                    return 0;
                }
                return await Orm.Update<TEntity>(items)
                     .Set(a => (a as IDeleteAuditEntity).IsDeleted, true)
                     .Set(a => (a as IDeleteAuditEntity).DeleteUserId, CurrentUser.Id)
                     .Set(a => (a as IDeleteAuditEntity).DeleteTime, DateTime.Now)
                     .ExecuteAffrowsAsync(cancellationToken);
            }

            return await base.DeleteAsync(predicate, cancellationToken);
        }
        public override async Task<TEntity> InsertOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            BeforeInsert(entity);
            BeforeUpdate(entity);
            await base.InsertOrUpdateAsync(entity, cancellationToken);
            return entity;
        }

    }
}
