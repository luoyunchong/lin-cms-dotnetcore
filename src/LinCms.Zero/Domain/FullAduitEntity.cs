using System;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain
{
    public abstract class EntityDto<T>
    {
        public T Id { get; set; }
    }

    public abstract class EntityDto : EntityDto<long>
    {

    }

    [Serializable]
    public class FullAduitEntity : FullAduitEntity<long>
    {

    }

    public class FullAduitEntity<T> : Entity<T>, IUpdateAuditEntity, ISoftDeleteAduitEntity, ICreateAduitEntity
    {
        /// <summary>
        /// 创建者ID
        /// </summary>
        public long? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeleteUserId { get; set; }
        public DateTime? DeleteTime { get; set; }
        public long? UpdateUserId { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public interface IUpdateAuditEntity
    {
        long? UpdateUserId { get; set; }
        DateTime UpdateTime { get; set; }
    }

    public interface ICreateAduitEntity
    {
        long? CreateUserId { get; set; }
        DateTime CreateTime { get; set; }
    }

    public interface ISoftDeleteAduitEntity
    {
        bool IsDeleted { get; set; }
        long? DeleteUserId { get; set; }
        DateTime? DeleteTime { get; set; }
    }

    public abstract class Entity<T> : IEntity<T>
    {
        [Column(IsPrimary = true,IsIdentity = true)]
        public T Id { get; set; }
    }

    [Serializable]
    public abstract class Entity : Entity<long>
    {

    }

    public interface IEntity<T>
    {
        T Id { get; set; }
    }

    public interface IEntity : IEntity<int>
    {

    }
}
