using System;
using LinCms.Application.Contracts;
using LinCms.Core.Security;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Application
{

    public abstract class AppService : IAppService
    {
        protected ICurrentUser CurrentUser { get; set; }

        protected AppService(IServiceProvider serviceProvider)
        {
            CurrentUser = serviceProvider.GetRequiredService<ICurrentUser>();
        }

    }
}
