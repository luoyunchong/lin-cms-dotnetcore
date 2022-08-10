using System.Data;
using FreeSql;

namespace LinCms.Middleware;

/// <summary>
/// 默认事务配置
/// </summary>
public class UnitOfWorkDefualtOptions
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