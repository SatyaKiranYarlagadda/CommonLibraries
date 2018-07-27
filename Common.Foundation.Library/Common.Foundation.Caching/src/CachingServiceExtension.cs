using Microsoft.Extensions.DependencyInjection;

namespace Common.Foundation.Caching
{
    public static class CachingServiceExtension
    {
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, string configuration, string instanceName)
        {
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = configuration;
                option.InstanceName = instanceName;
            });

            return services;
        }
    }
}
