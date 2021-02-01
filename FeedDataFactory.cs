using System;
using Quartz;
using Quartz.Spi;

namespace QuartzJobDemo
{
    public class FeedDataFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public FeedDataFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            return (IJob)_serviceProvider.GetService(jobDetail.JobType);
        }

        public void ReturnJob(IJob job) { }
    }
}