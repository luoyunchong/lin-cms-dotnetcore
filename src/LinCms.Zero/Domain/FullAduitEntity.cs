using System;
using System.Collections.Generic;
using System.Reflection;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain
{
    public abstract class EntityDto<T>
    {
        public T Id { get; set; }
    }

    public abstract class EntityDto : EntityDto<int>
    {

    }

    [Serializable]
    public class FullAduitEntity : FullAduitEntity<int>
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
        /// <returns>True, if this entity is transient</returns>
        public virtual bool IsTransient()
        {
            if (EqualityComparer<T>.Default.Equals(Id, default(T)))
            {
                return true;
            }

            //Workaround for EF Core since it sets int/long to min value when attaching to dbcontext
            if (typeof(T) == typeof(int))
            {
                return Convert.ToInt32(Id) <= 0;
            }

            if (typeof(T) == typeof(long))
            {
                return Convert.ToInt64(Id) <= 0;
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is Entity<T>))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            Entity<T> other = (Entity<T>)obj;
            if (IsTransient() && other.IsTransient())
            {
                return false;
            }

            //Must have a IS-A relation of types or must be same type
            Type typeOfThis = GetType();
            Type typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }
    }

    [Serializable]
    public abstract class Entity : Entity<int>
    {

    }

    public interface IEntity<T>
    {
        T Id { get; set; }
        /// <summary>
        /// Checks if this entity is transient (not persisted to database and it has not an <see cref="Id"/>).
        /// </summary>
        /// <returns>True, if this entity is transient</returns>
        bool IsTransient();
    }

    public interface IEntity : IEntity<int>
    {

    }
}
