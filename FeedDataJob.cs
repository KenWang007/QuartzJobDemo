using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace QuartzJobDemo
{
    [DisallowConcurrentExecution]
    public class FeedDataJob : IJob
    {
        private readonly ILogger<FeedDataJob> _logger;

        public FeedDataJob(ILogger<FeedDataJob> logger)
        {
            _logger = logger;
        }
        
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("------------FeedEsDataJob executed!---------------------");
            return Task.CompletedTask;
        }
    }
}