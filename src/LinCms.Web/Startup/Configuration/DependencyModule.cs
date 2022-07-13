using System;
using System.Linq;
using System.Reflection;
using Autofac;
using LinCms.Dependency;

namespace LinCms.Startup.Configuration
{
    /// <summary>
    /// 接口注入
    /// </summary>
    public class DependencyModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Assembly[] _currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName.Contains("LinCms.")).ToArray();

            //每次调用，都会重新实例化对象；每次请求都创建一个新的对象；
            bool TransientPredicate(Type t) => !t.IsDefined(typeof(DisableConventionalRegistrationAttribute), true) && typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType;

            builder.RegisterAssemblyTypes(_currentAssemblies)
                .Where(TransientPredicate)
                .AsImplementedInterfaces().InstancePerDependency();

            //同一个Lifetime生成的对象是同一个实例
            bool ScopePredicate(Type t) => !t.IsDefined(typeof(DisableConventionalRegistrationAttribute), true) && typeof(IScopedDependency).GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType;
            builder.RegisterAssemblyTypes(_currentAssemblies)
                .Where(ScopePredicate)
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            //单例模式，每次调用，都会使用同一个实例化的对象；每次都用同一个对象；
            bool SingletonPredicate(Type t) => !t.IsDefined(typeof(DisableConventionalRegistrationAttribute), true) && typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType;
            builder.RegisterAssemblyTypes(_currentAssemblies)
                .Where(SingletonPredicate)
                .AsImplementedInterfaces().SingleInstance();

        }
    }
}
