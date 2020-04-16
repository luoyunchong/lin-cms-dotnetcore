using System;
using AutoMapper;
using LinCms.Application.Contracts;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LinCms.Application
{
    public abstract class ApplicationService : IApplicationService
    {
        public IServiceProvider ServiceProvider { get; set; }

        protected readonly object ServiceProviderLock = new object();

        protected TService LazyGetRequiredService<TService>(ref TService reference)
            => LazyGetRequiredService(typeof(TService), ref reference);

        protected TRef LazyGetRequiredService<TRef>(Type serviceType, ref TRef reference)
        {
            if (reference == null)
            {
                lock (ServiceProviderLock)
                {
                    if (reference == null)
                    {
                        reference = (TRef) ServiceProvider.GetRequiredService(serviceType);
                    }
                }
            }

            return reference;
        }

        private ICurrentUser _currentUser;
        public ICurrentUser CurrentUser => LazyGetRequiredService(ref _currentUser);

        private IMapper _mapper;
        public IMapper Mapper => LazyGetRequiredService(ref _mapper);
        
        public ILoggerFactory LoggerFactory => LazyGetRequiredService(ref _loggerFactory);
        private ILoggerFactory _loggerFactory;
        
        protected ILogger Logger => _lazyLogger.Value;
        private Lazy<ILogger> _lazyLogger => new Lazy<ILogger>(() => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance, true);
        
        public IAuthorizationService AuthorizationService => LazyGetRequiredService(ref _authorizationService);
        private IAuthorizationService _authorizationService;
    }

}