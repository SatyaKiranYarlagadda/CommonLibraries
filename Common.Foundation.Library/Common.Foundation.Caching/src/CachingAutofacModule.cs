using Autofac;

namespace Common.Foundation.Caching
{
    public class CachingAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CacheUtils>().AsImplementedInterfaces().InstancePerDependency();
        }
    }
}
