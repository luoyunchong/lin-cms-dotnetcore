using System;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain
{
    public abstract class EntityDto<T>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
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
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 删除人id
        /// </summary>
        public long? DeleteUserId { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 最后修改人Id
        /// </summary>
        public long? UpdateUserId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }

    public interface IUpdateAuditEntity
    {
        /// <summary>
        /// 最后修改人Id
        /// </summary>
        long? UpdateUserId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        DateTime UpdateTime { get; set; }
    }

    public interface ICreateAduitEntity
    {
        /// <summary>
        /// 创建者ID
        /// </summary>
        long? CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }
    }

    public interface ISoftDeleteAduitEntity
    {
        /// <summary>
        /// 是否删除
        /// </summary>
         bool IsDeleted { get; set; }
        /// <summary>
        /// 删除人id
        /// </summary>
         long? DeleteUserId { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
         DateTime? DeleteTime { get; set; }
    }

    public abstract class Entity<T> : IEntity<T>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Column(IsPrimary = true,IsIdentity = true)]
        public T Id { get; set; }
    }

    [Serializable]
    public abstract class Entity : Entity<long>
    {

    }

    public interface IEntity<T>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        T Id { get; set; }
    }

    public interface IEntity : IEntity<int>
    {

    }
}
