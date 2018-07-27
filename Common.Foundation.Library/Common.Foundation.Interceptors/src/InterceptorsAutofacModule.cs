using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Castle.DynamicProxy;
using CorrelationId;
using FluentValidation;
using Common.Foundation.Interceptors.src;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Foundation.Interceptors
{
    public class InterceptorsAutofacModule : Autofac.Module
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly Type _validatorAssemblyType;

        public InterceptorsAutofacModule(IServiceCollection serviceCollection, Type validatorAssemblyType)
        {
            _serviceCollection = serviceCollection;
            _validatorAssemblyType = validatorAssemblyType;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<LoggingInterceptor>>();
            builder.Register(c => new LoggingInterceptor(logger))
                .Named<IInterceptor>("log-calls");

            var httpContextInterceptorLogger = serviceProvider.GetService<ILogger<HttpContextInterceptor>>();
            var correlationContextAccessor = serviceProvider.GetService<ICorrelationContextAccessor>();
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            builder.Register(c => new HttpContextInterceptor(httpContextInterceptorLogger, httpContextAccessor, correlationContextAccessor))
                .Named<IInterceptor>("http-context");

            var tempBuilder = new ContainerBuilder();
            tempBuilder
                .RegisterAssemblyTypes(_validatorAssemblyType.Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();
            var tempContainer = tempBuilder.Build();
            var validators = tempContainer.Resolve<IEnumerable<IValidator>>();

            builder.Register(c => new ValidationInterceptor(validators.ToArray()))
                .Named<IInterceptor>("validate-request");

            var cacheLogger = serviceProvider.GetService<ILogger<CachingInterceptor>>();
            var cache = serviceProvider.GetService<IDistributedCache>();
            builder.Register(c => new CachingInterceptor(cacheLogger, cache))
                .Named<IInterceptor>("cache-response");
        }
    }
}
