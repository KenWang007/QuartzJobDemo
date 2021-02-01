using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;

namespace QuartzJobDemo
{
    public class JobHostedService : IHostedService
    {
        private readonly ILogger<JobHostedService> _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly JobMetadata _jobMetadata;
        private IScheduler _scheduler;

        public JobHostedService(IServiceProvider appApplicationServices)
        {
            _logger = appApplicationServices.GetService<ILogger<JobHostedService>>();
            _schedulerFactory = appApplicationServices.GetService<ISchedulerFactory>();
            _jobFactory = appApplicationServices.GetService<IJobFactory>();
            _jobMetadata = appApplicationServices.GetService<JobMetadata>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Job start in JobHostedService..........." + _jobMetadata.CronExpression + _jobMetadata.JobName);
            _scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            _scheduler.JobFactory = _jobFactory;
            var job = CreateJob();
            var jobTrigger = CreateJobTrigger();
            
            _logger.LogInformation("Job and trigger created in JobHostedService...........");
            
            await _scheduler.ScheduleJob(job, jobTrigger, cancellationToken);
            await _scheduler.Start(cancellationToken);
            _logger.LogInformation("Job started to execute...........");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
        
        private ITrigger CreateJobTrigger()
        {
            var jobTrigger = TriggerBuilder
                .Create()
                .WithIdentity(_jobMetadata.JobId.ToString())
                .WithCronSchedule(_jobMetadata.CronExpression)
                .WithDescription(_jobMetadata.JobName)
                .Build();

            return jobTrigger;
        }
        
        private IJobDetail CreateJob()
        {
            var jobDetail = JobBuilder
                .Create(_jobMetadata.JobType)
                .WithIdentity(_jobMetadata.JobId.ToString())
                .WithDescription(_jobMetadata.JobName)
                .Build();

            return jobDetail;
        }
    }
}