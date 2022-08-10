using System;
using System.Data;
using FreeSql;

namespace LinCms.Aop.Attributes
{
    /// <summary>
    /// 事务
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionalAttribute : Attribute
    {
        /// <summary>
        /// 事务传播方式
        /// </summary>
        public Propagation? Propagation { get; set; }

        /// <summary>
        /// 事务隔离级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }
    }
}