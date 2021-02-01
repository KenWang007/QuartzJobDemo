using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace QuartzJobDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            services.AddHealthChecks();
            services.AddSingleton(scheduler);
            services.AddSingleton<IJobFactory, FeedDataFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<FeedDataJob>();
            
            services.AddSingleton(new JobMetadata(Guid.NewGuid(), typeof(FeedDataJob),"FeedEsData Job", "0/10 * * * * ?"));

            services.AddHostedService<JobHostedService>();
            
            services.AddHealthChecks()
                .AddCheck<JobHealthCheck>("Job Health Check");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            app.UseRouting();
            
            Console.WriteLine("Configure been called!");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
