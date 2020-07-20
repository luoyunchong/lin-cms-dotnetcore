using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities
{
    public interface IEntityDto
    {
    }

    public interface IEntityDto<TKey> : IEntityDto
    {
        TKey Id { get; set; }
    }

    public abstract class EntityDto<TKey> : IEntityDto<TKey>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public TKey Id { get; set; }
    }

    public abstract class EntityDto : EntityDto<long>
    {
    }

    [Serializable]
    public class FullAduitEntity : FullAduitEntity<long>
    {
    }

    public class FullAduitEntity<T> : Entity<T>, IUpdateAuditEntity, IDeleteAduitEntity, ICreateAduitEntity
    {
        /// <summary>
        /// 创建者ID
        /// </summary>

        [Column(Position = -7)]
        public long CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Position = -6)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [Column(Position = -5)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除人id
        /// </summary>
        [Column(Position = -4)]
        public long? DeleteUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [Column(Position = -3)]
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 最后修改人Id
        /// </summary>
        [Column(Position = -2)]
        public long? UpdateUserId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column(Position = -1)]
        public DateTime? UpdateTime { get; set; }
    }


    public interface ICreateAduitEntity
    {
        /// <summary>
        /// 创建者ID
        /// </summary>
        long CreateUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }
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
        DateTime? UpdateTime { get; set; }
    }

    public interface IDeleteAduitEntity
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

    //[Index("uk_id_index", "id asc", true)]
    public abstract class Entity<T> : IEntity<T>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true, Position = 1)]
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

    public interface IEntity : IEntity<long>
    {
    }
}