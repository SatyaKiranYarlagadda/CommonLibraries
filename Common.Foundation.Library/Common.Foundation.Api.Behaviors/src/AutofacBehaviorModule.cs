using Autofac;
using FluentValidation;
using MediatR;
using System;
using System.Reflection;

namespace Common.Foundation.Api.Behaviors
{
    public class AutofacBehaviorModule : Autofac.Module
    {
        private readonly Type _validatorAssemblyType;

        public AutofacBehaviorModule(Type validatorAssemblyType)
        {
            _validatorAssemblyType = validatorAssemblyType;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder
                .RegisterAssemblyTypes(_validatorAssemblyType.GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            
            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

        }
    }
}
