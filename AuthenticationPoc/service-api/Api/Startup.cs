﻿using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using CorrelationId;
using FluentValidation.AspNetCore;
using HCF.Claims.Common.Utils;
using service_api.Domain.CommandHandlers;
using service_api.Domain.QueryHandlers;
using service_api.Domain.Validators;
using HCF.Common.Foundation.Api.Behaviors;
using HCF.Common.Foundation.Api.GlobalFilters;
using HCF.Common.Foundation.Api.HealthCheck;
using HCF.Common.Foundation.Api.Http.DelegateHandlers;
using HCF.Common.Foundation.Api.Logging;
using HCF.Common.Foundation.Caching;
using HCF.Common.Foundation.CQRSExtensions;
using HCF.Common.Foundation.FaultTolerance;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using service_api.Api.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace service_api.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(InjectionHelper.GetHcfAssembliesWithAutoMapperProfile(Assembly.GetExecutingAssembly().Location));
            services.AddMediatR(typeof(Startup));

            services.AddSingleton<AvailabilityHealthCheck>();
            services.AddHealthChecks(c =>
            {
                c.AddHealthCheckGroup("Availability",
                    group => group.AddCheck<AvailabilityHealthCheck>("Availability"),
                    CheckStatus.Unhealthy
                );
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.Ntlm);

            services.AddCorrelationId();
            services.AddCors(Configuration);

            services.Configure<TokenManagement>(options => Configuration.GetSection("TokenManagement").Bind(options));
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            var secret = Encoding.UTF8.GetBytes(token.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(secret)
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireClaim("User"));
                options.AddPolicy("MedicalUser", policy => policy.RequireClaim("IsMedical", "True"));
                options.AddPolicy("HospitalUser", policy => policy.RequireClaim("IsHospital", "True"));
                options.AddPolicy("AncillaryUser", policy => policy.RequireClaim("IsAncillary", "True"));
            });

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info { Title = "Service.Api", Version = "v1", });
                s.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                s.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
                s.DescribeAllEnumsAsStrings();
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ResponseTimeLogFilter));
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.Filters.Add(typeof(ValidateModelStateFilter));
            }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
              .AddJsonOptions(o => o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()))
              .AddControllersAsServices()
              .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var cacheConfig = Configuration["Caching:Endpoint"] + ":" + Configuration["Caching:Port"];
            services.AddRedisCaching(cacheConfig, Configuration["Caching:InstanceName"]);

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigins"));
            });            

            services.AddTransient<ResponseTimeHandler>();
            services.AddTransient<LoggingHandler>();
            services.AddTransient<DefaultRequestHeadersHandler>();

            // Add named Http client to communicate with external REST APIs
            // Uncomment this portion if there is a need to add Http client and fix the properties accordingly.
            //services.AddResilientHttpClient("ClientName", client =>
            //{
            //    client.BaseAddress = new Uri(Configuration["ServiceEndpoint"]);
            //    client.DefaultRequestHeaders.Add("Accept", "application/json");
            //});


            var builder = new ContainerBuilder();

            builder
                .RegisterModule(new CachingAutofacModule())
                .RegisterModule(new PipelineBehaviorsAutofacModule(typeof(GetApiInfoQueryValidator)))
                .RegisterModule(new CQRSExtensionsAutofacModule(typeof(TestPipelineCommandHandler), typeof(GetApiInfoQueryHandler)))
                .RegisterModule(new RetryableOperationAutofacModule());

            builder.Populate(services);
            InjectionHelper.RegisterTypesFromHcfAssemblies(Assembly.GetExecutingAssembly().Location, builder);
            var container = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();

            app.UseCorrelationId(new CorrelationIdOptions
            {
                UseGuidForCorrelationId = true
            });

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(u => 
            {
                u.SwaggerEndpoint("/swagger/v1/swagger.json", "Service API");                
            });

            app.UseLoggingContextMiddleware();
            app.UseRequestResponseLoggingMiddleware();
            app.UseEchoHeadersInResponseMiddleware();
            app.UseMvc();
        }
    }
}
