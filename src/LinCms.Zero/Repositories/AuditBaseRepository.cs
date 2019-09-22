using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Zero.Domain;
using LinCms.Zero.Security;

namespace LinCms.Zero.Repositories
{
    /// <summary>
    /// 审计仓储：实现如果实体类
    /// 继承了ICreateAduitEntity  则自动增加创建时间/人信息
    /// 继承了IUpdateAuditEntity，更新时，修改更新时间/人
    /// 继承了ISoftDeleteAduitEntity，删除时，自动改成软删除。仅注入此仓储或继承此仓储的实现才能实现如上功能。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AuditBaseRepository<T> : BaseRepository<T> where T : class, new()
    {
        private readonly ICurrentUser _currentUser;
        public AuditBaseRepository(ICurrentUser currentUser, IFreeSql fsql, Expression<Func<T, bool>> filter = null, Func<string, string> asTable = null) : base(fsql, filter, asTable)
        {
            _currentUser = currentUser;
        }

        private void BeforeInsert(T entity)
        {
            if (!(entity is ICreateAduitEntity e)) return;
            e.CreateTime = DateTime.Now;
            e.CreateUserId = _currentUser.Id;

            if (!(entity is IUpdateAuditEntity updateAuditEntity)) return;
            updateAuditEntity.UpdateTime = DateTime.Now;
            updateAuditEntity.UpdateUserId = _currentUser.Id; ;
        }

        public override T Insert(T entity)
        {
            BeforeInsert(entity);
            return base.Insert(entity);
        }

        public override Task<T> InsertAsync(T entity)
        {
            this.BeforeInsert(entity);
            return base.InsertAsync(entity);
        }

        public override List<T> Insert(IEnumerable<T> entitys)
        {
            foreach (T entity in entitys)
            {
                BeforeInsert(entity);
            }

            return base.Insert(entitys);
        }

        public override Task<List<T>> InsertAsync(IEnumerable<T> entitys)
        {
            foreach (T entity in entitys)
            {
                BeforeInsert(entity);
            }
            return base.InsertAsync(entitys);
        }

        private void BeforeUpdate(T entity)
        {
            if (!(entity is IUpdateAuditEntity e)) return;
            e.UpdateTime = DateTime.Now;
            e.UpdateUserId = _currentUser.Id;
        }

        public new int Update(T entity)
        {
            BeforeUpdate(entity);
            return base.Update(entity);
        }

        public new Task<int> UpdateAsync(T entity)
        {
            BeforeUpdate(entity);
            return base.UpdateAsync(entity);
        }

        public new int Update(IEnumerable<T> entitys)
        {
            foreach (var entity in entitys)
            {
                this.BeforeUpdate(entity);
            }
            return base.Update(entitys);
        }

        public new Task<int> UpdateAsync(IEnumerable<T> entitys)
        {
            foreach (var entity in entitys)
            {
                BeforeUpdate(entity);
            }
            return base.UpdateAsync(entitys);
        }

        public new int Delete(T entity)
        {
            if (entity is ISoftDeleteAduitEntity)
            {
                return Orm.Update<T>(entity)
                           .Set(a => (a as ISoftDeleteAduitEntity).IsDeleted, true)
                           .Set(a => (a as ISoftDeleteAduitEntity).DeleteUserId, _currentUser.Id)
                           .Set(a => (a as ISoftDeleteAduitEntity).DeleteTime, DateTime.Now)
                           .ExecuteAffrows();
            }

            return base.Delete(entity);
        }

        public new int Delete(IEnumerable<T> entitys)
        {
            if (entitys.Any())
            {
                Attach(entitys);
                foreach (T x1 in entitys)
                {
                    if (x1 is ISoftDeleteAduitEntity softDelete)
                    {
                        softDelete.DeleteUserId = _currentUser.Id;
                        softDelete.DeleteTime = DateTime.Now;
                        softDelete.IsDeleted = true;
                    }
                }

                return Update(entitys);
            }

            return base.Delete(entitys);
        }


        public new Task<int> DeleteAsync(IEnumerable<T> entitys)
        {
            if (entitys.Any())
            {
                Attach(entitys);
                foreach (T x1 in entitys)
                {
                    if (x1 is ISoftDeleteAduitEntity softDelete)
                    {
                        softDelete.DeleteUserId = _currentUser.Id;
                        softDelete.DeleteTime = DateTime.Now;
                        softDelete.IsDeleted = true;
                    }
                }
                return UpdateAsync(entitys);
            }
            return base.DeleteAsync(entitys);
        }

        public new async Task<int> DeleteAsync(T entity)
        {
            if (entity is ISoftDeleteAduitEntity)
            {
                return await Orm.Update<T>(entity)
                    .Set(a => (a as ISoftDeleteAduitEntity).IsDeleted, true)
                    .Set(a => (a as ISoftDeleteAduitEntity).DeleteUserId, _currentUser.Id)
                    .Set(a => (a as ISoftDeleteAduitEntity).DeleteTime, DateTime.Now)
                    .ExecuteAffrowsAsync();
            }

            return base.Delete(entity);
        }
        public new int Delete(Expression<Func<T, bool>> predicate)
        {
            if (typeof(ISoftDeleteAduitEntity).IsAssignableFrom(typeof(T)))
            {
                List<T> items = Orm.Select<T>().Where(predicate).ToList();
                return Orm.Update<T>(items)
                    .Set(a => (a as ISoftDeleteAduitEntity).IsDeleted, true)
                    .Set(a => (a as ISoftDeleteAduitEntity).DeleteUserId, _currentUser.Id)
                    .Set(a => (a as ISoftDeleteAduitEntity).DeleteTime, DateTime.Now)
                    .ExecuteAffrows();
            }

            return base.Delete(predicate);
        }

        public new async Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            if (typeof(ISoftDeleteAduitEntity).IsAssignableFrom(typeof(T)))
            {
                List<T> items = Orm.Select<T>().Where(predicate).ToList();
                return await Orm.Update<T>(items)
                     .Set(a => (a as ISoftDeleteAduitEntity).IsDeleted, true)
                     .Set(a => (a as ISoftDeleteAduitEntity).DeleteUserId, _currentUser.Id)
                     .Set(a => (a as ISoftDeleteAduitEntity).DeleteTime, DateTime.Now)
                     .ExecuteAffrowsAsync();
            }

            return await base.DeleteAsync(predicate);
        }

    }
}
