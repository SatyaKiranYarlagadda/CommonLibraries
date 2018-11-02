using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Foundation.Repositories.ServiceExtenisons
{
    public static class UnitOfWorkServiceExtension
    {
        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
            services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync<TContext>>();
            services.AddScoped<IUnitOfWorkAsync<TContext>, UnitOfWorkAsync<TContext>>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext1, TContext2>(this IServiceCollection services)
            where TContext1 : DbContext
            where TContext2 : DbContext
        {
            services.AddScoped<IUnitOfWork<TContext1>, UnitOfWork<TContext1>>();
            services.AddScoped<IUnitOfWork<TContext2>, UnitOfWork<TContext2>>();

            services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync<TContext1>>();
            services.AddScoped<IUnitOfWorkAsync<TContext1>, UnitOfWorkAsync<TContext1>>();
            services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync<TContext2>>();
            services.AddScoped<IUnitOfWorkAsync<TContext2>, UnitOfWorkAsync<TContext2>>();
            return services;
        }

        public static IServiceCollection AddUnitOfWork<TContext1, TContext2, TContext3>(
            this IServiceCollection services)
            where TContext1 : DbContext
            where TContext2 : DbContext
            where TContext3 : DbContext
        {
            services.AddScoped<IUnitOfWork<TContext1>, UnitOfWork<TContext1>>();
            services.AddScoped<IUnitOfWork<TContext2>, UnitOfWork<TContext2>>();
            services.AddScoped<IUnitOfWork<TContext3>, UnitOfWork<TContext3>>();

            services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync<TContext1>>();
            services.AddScoped<IUnitOfWorkAsync<TContext1>, UnitOfWorkAsync<TContext1>>();
            services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync<TContext2>>();
            services.AddScoped<IUnitOfWorkAsync<TContext2>, UnitOfWorkAsync<TContext2>>();
            services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync<TContext3>>();
            services.AddScoped<IUnitOfWorkAsync<TContext3>, UnitOfWorkAsync<TContext3>>();

            return services;
        }
    }
}
