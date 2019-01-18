using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundServices;
using BackgroundServices.Models;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace BackgroundProcessPoc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info { Title = "BackgroundProcessPoc", Version = "v1", });
                s.DescribeAllEnumsAsStrings();
            });

            services.AddTransient<IDataService, DataService>();
            services.AddDbContext<BackgroundTasksContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BackgroundTaskDatabase")));

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("BackgroundTaskDatabase")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(u => { u.SwaggerEndpoint("../swagger/v1/swagger.json", "BackgroundProcessPoc"); });

            GlobalConfiguration.Configuration.UseActivator(new BackgroundJobActivator(serviceProvider));

            app.UseHangfireServer();
            app.UseHangfireDashboard("/backgroundJobs");
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
