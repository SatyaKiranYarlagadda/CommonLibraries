using Common.Foundation.Api.Http.DelegateHandlers;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace Common.Foundation.Api.Http
{
    public static class HttpClientFactoryServiceExtensions
    {
        public static IServiceCollection AddResilientHttpClient(this IServiceCollection services, string clientName, Action<HttpClient> clientBuilder)
        {
            services.AddHttpClient(clientName, clientBuilder)
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }))
                .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                ))
                .AddHttpMessageHandler<ResponseTimeHandler>()
                .AddHttpMessageHandler<DefaultRequestHeadersHandler>();

            return services;
        }

        public static IServiceCollection AddResilientHttpClient<TClient>(this IServiceCollection services, string clientName, Action<HttpClient> clientBuilder) where TClient : class
        {
            services.AddHttpClient<TClient>(clientName, clientBuilder)
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }))
                .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                ))
                .AddHttpMessageHandler<ResponseTimeHandler>()
                .AddHttpMessageHandler<DefaultRequestHeadersHandler>();

            return services;
        }

        public static IServiceCollection AddResilientHttpClientForNonIdempotentBackend(this IServiceCollection services, string clientName, Action<HttpClient> clientBuilder)
        {
            var retryPolicy = HttpPolicyExtensions
                                .HandleTransientHttpError()
                                .WaitAndRetryAsync(new[]
                                {
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(5),
                                    TimeSpan.FromSeconds(10)
                                });

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            services.AddHttpClient(clientName, clientBuilder)
                .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
                .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                ))
                .AddHttpMessageHandler<ResponseTimeHandler>()
                .AddHttpMessageHandler<DefaultRequestHeadersHandler>();

            return services;
        }

        public static IServiceCollection AddResilientHttpClientForNonIdempotentBackend<TClient>(this IServiceCollection services, string clientName, Action<HttpClient> clientBuilder) where TClient : class
        {
            var retryPolicy = HttpPolicyExtensions
                                .HandleTransientHttpError()
                                .WaitAndRetryAsync(new[]
                                {
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(5),
                                    TimeSpan.FromSeconds(10)
                                });

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();

            services.AddHttpClient<TClient>(clientName, clientBuilder)
                .AddPolicyHandler(request => request.Method == HttpMethod.Get ? retryPolicy : noOpPolicy)
                .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                ))
                .AddHttpMessageHandler<ResponseTimeHandler>()
                .AddHttpMessageHandler<DefaultRequestHeadersHandler>();

            return services;
        }

        public static IServiceCollection AddHttpClientWithDefaultHandlers(this IServiceCollection services, string clientName, Action<HttpClient> clientBuilder)
        {
            services.AddHttpClient(clientName, clientBuilder)
                .AddHttpMessageHandler<ResponseTimeHandler>()
                .AddHttpMessageHandler<DefaultRequestHeadersHandler>();

            return services;
        }

        public static IServiceCollection AddHttpClientWithDefaultHandlers<TClient>(this IServiceCollection services, string clientName, Action<HttpClient> clientBuilder) where TClient : class
        {
            services.AddHttpClient<TClient>(clientName, clientBuilder)
                .AddHttpMessageHandler<ResponseTimeHandler>()
                .AddHttpMessageHandler<DefaultRequestHeadersHandler>();

            return services;
        }
    }
}
