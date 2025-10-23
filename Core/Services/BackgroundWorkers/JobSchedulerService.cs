using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using zervemedata.Data;
using zervemedata.Data.Entities;
using zervemedata.Data.Enumerations;

namespace zervemedata.Core.Services.BackgroundWorkers
{
    public class JobSchedulerService(
        IServiceProvider serviceProvider,
        ILogger<JobSchedulerService> logger)
        : BackgroundService
    {
        private const uint TIMEOUT = 30;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("JobSchedulerService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ZervemedataDbContext>();

                    var now = DateTime.UtcNow;

                    var validJobsToCheck = await dbContext.JobEntities
                        .Where(j => j.JobFreqType != null && j.JobFreqType.ScheduleType != ScheduleType.OneOff &&
                                    j.NextScheduledRunDateTime <= now &&
                                    j.JobStatusType == JobStatusType.Completed)
                        .ToListAsync(cancellationToken: stoppingToken);

                    if (!validJobsToCheck.Any())
                    {
                        logger.LogInformation("No jobs to queue.");
                        await Task.Delay(TimeSpan.FromSeconds(TIMEOUT), stoppingToken);
                        continue;
                    }

                    foreach (var js in validJobsToCheck.OfType<JobEntity>())
                    {
                        js.JobStatusType = JobStatusType.Queued;
                        logger.LogInformation($"Job Schedule ID {js.Id} queued.");
                    }

                    if (validJobsToCheck.Any())
                    {
                        await dbContext.SaveChangesAsync(stoppingToken);
                        logger.LogInformation($"Queued {validJobsToCheck.Count} job(s) at {now}.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while scheduling jobs.");
                }

                await Task.Delay(TimeSpan.FromSeconds(TIMEOUT), stoppingToken);
            }

            logger.LogInformation("JobSchedulerService is stopping.");
        }
    }
}