using Autofac;
using MediatR;
using System;
using System.Reflection;

namespace Common.Foundation.DomainModel
{
    public class AutofacDomainModelModule : Autofac.Module
    {
        private readonly Type _commandHandlerAssemblyType;
        private readonly Type _queryHandlerAssemblyType;

        public AutofacDomainModelModule(Type commandHandlerAssemblyType, Type queryHandlerAssemblyType)
        {
            _commandHandlerAssemblyType = commandHandlerAssemblyType;
            _queryHandlerAssemblyType = queryHandlerAssemblyType;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
               .AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(_commandHandlerAssemblyType.GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register all the Query classes (they implement IRequestHandler) in assembly holding the Queries
            builder.RegisterAssemblyTypes(_queryHandlerAssemblyType.GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }
    }
}
