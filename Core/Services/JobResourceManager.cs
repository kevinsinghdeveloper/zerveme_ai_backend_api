using zervemedata.Data.DataModels.UpdateMeta;
using zervemedata.Data.Entities;
using zervemedata.Data.Enumerations;

namespace zervemedata.Core.Services
{
    using zervemedata.Data.DataModels.CreateMeta;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using zervemedata.Data;
    using zervemedata.Data.AuthData;
    using Google.Apis.Util;
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Core.Extensions;
    using zervemedata.Data.DataModels.Data;
    using zervemedata.Data.Entities.Keys;
    using IJobResourceManager = zervemedata.Core.Contracts.Abstractions.IJobResourceManager<
        zervemedata.Data.DataModels.Data.JobInfo,
        zervemedata.Data.DataModels.Data.JobFreqTypeInfo,
        zervemedata.Data.Entities.Keys.JobKey,
        zervemedata.Data.DataModels.CreateMeta.JobCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.JobUpdateMetaData,
        zervemedata.Data.DataModels.UpdateMeta.JobScheduleUpdateMetaData
    >;

    public class JobResourceManager(
        ZervemedataDbContext dbContext) : IJobResourceManager
    {
        private async Task<(ApplicationUser user, OrgUserEntity orgUser, OrganizationEntity organization)>
            ValidateUserAndOrg(string userId)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found.");

            var orgUser = await dbContext.OrgUserEntities.FirstOrDefaultAsync(ou => ou.UserId == user.Id);
            if (orgUser == null) throw new Exception("User not found in organization.");

            var organization =
                await dbContext.OrganizationEntities.FirstOrDefaultAsync(o => o.Id == orgUser.OrganizationId);
            if (organization == null) throw new Exception("Organization not found.");

            return (user, orgUser, organization);
        }

        private DateTime GenerateNextRunTime(ScheduleType scheduleType, DateTime? currentDateTime = null)
        {
            var baseTime = currentDateTime ?? DateTime.Now;

            return scheduleType switch
            {
                ScheduleType.OneOff => baseTime,
                ScheduleType.Daily => baseTime.AddDays(1),
                ScheduleType.Weekly => baseTime.AddDays(7),
                ScheduleType.Monthly => baseTime.AddMonths(1),
                // ScheduleType.Quarterly => baseTime.AddMonths(3),
                // ScheduleType.Yearly => baseTime.AddYears(1),
                _ => throw new ArgumentException("Invalid schedule type", nameof(scheduleType))
            };
        }

        public async Task<JobInfo> Create(JobCreationMetaData creationMetaData)
        {
            var (user, orgUser, _) = await ValidateUserAndOrg(creationMetaData.CurrentUser);

            var jobFreqType = await dbContext.JobFreqTypeEntities
                .FirstOrDefaultAsync(jf => jf.Id == creationMetaData.JobFreqTypeId);

            if (jobFreqType == null)
                throw new Exception("Job frequency type not found.");

            var nextRunDateTime = GenerateNextRunTime(jobFreqType.ScheduleType);

            // var jobSchedule = new JobScheduleEntity()
            // {
            //     StartScheduledRunDateTime = DateTime.Now,
            //     NextScheduledRunDateTime = nextRunDateTime,
            //     JobStatusType = JobStatusType.Queued,
            //     Created = DateTime.Now,
            //     Updated = DateTime.Now,
            //     CreatedUser = user,
            //     UpdatedUser = user,
            // };

            // find schedule type entity
            // var jobFreqType = await dbContext.JobFreqTypeEntities
            //     .FirstOrDefaultAsync(jf => jf.ScheduleType == creationMetaData.ScheduleType);
            
            var job = new JobEntity()
            {
                JobFreqType = jobFreqType,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                CreatedUser = user,
                UpdatedUser = user,
                RecentScheduledRunDateTime = null,
                NextScheduledRunDateTime = nextRunDateTime,
                JobStatusType = JobStatusType.Queued
            };

            await dbContext.JobEntities.AddAsync(job);
            // await dbContext.JobScheduleEntities.AddAsync(jobSchedule);

            await dbContext.SaveChangesAsync();

            return job.ToDataModel();
        }

        public async Task<JobInfo> Update(JobUpdateMetaData updateMetaData)
        {
            var (user, orgUser, _) = await ValidateUserAndOrg(updateMetaData.CurrentUser);

            var job = await dbContext.JobEntities
                // .Include(js => js.JobScheduleEntity)
                .FirstOrDefaultAsync(j => j.Id == updateMetaData.Id);

            if (job == null) throw new KeyNotFoundException("Job not found.");

            var jobFreqType = await dbContext.JobFreqTypeEntities
                .FirstOrDefaultAsync(jf => jf.Id == updateMetaData.JobFreqTypeId);

            if (jobFreqType == null)
                throw new Exception("Job frequency type not found.");

            if (jobFreqType == null)
                throw new Exception("Job frequency type not found.");

            // var jobSchedule =
            //     await dbContext.JobScheduleEntities.FirstOrDefaultAsync(js => js.Id == job.JobScheduleId);

            // if (jobSchedule == null)
            // {
            //     // create
            //     jobSchedule = new JobScheduleEntity()
            //     {
            //         StartScheduledRunDateTime = DateTime.Now,
            //         NextScheduledRunDateTime = GenerateNextRunTime(jobFreqType.ScheduleType),
            //         JobStatusType = JobStatusType.Queued,
            //         Created = DateTime.Now,
            //         Updated = DateTime.Now,
            //         CreatedUser = user,
            //         UpdatedUser = user,
            //     };
            // }
            
            job.JobFreqType = jobFreqType;
            job.Updated = DateTime.Now;
            job.UpdatedUser = user;
            // job.JobScheduleEntity = jobSchedule ?? job.JobScheduleEntity;
            return job.ToDataModel();
        }

        public async Task<IEnumerable<JobInfo>> GetAll(JobKey key)
        {
            var (_, _, _) = await ValidateUserAndOrg(key.CurrentUser);
    
            // TODO get all jobs for a particular report -- optional
            // Need to make it easy for the ETL to read
            // We should make the arg broader, if we want eg. queued scheduled jobs or something

            // TODO NEXT

            var jobs = await dbContext.JobEntities
                // .Include(js => js.JobScheduleEntity)
                .Where(j => j.Deleted == null) // Only include non-deleted jobs
                .ToListAsync();

            return jobs.Select(x => x.ToDataModel());
        }

        public async Task<JobInfo?> Get(JobKey key)
        {
            var (user, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            var job = await dbContext.JobEntities
                // .Include(js => js.JobScheduleEntity)
                .FirstOrDefaultAsync(j => j.Id == key.Id && j.Deleted == null); // Only include non-deleted job

            return job?.ToDataModel();
        }

        public async Task<IEnumerable<JobFreqTypeInfo>> GetAllJobFreqTypes(JobKey key)
        {
            var (_, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            var jobFreqTypes = await dbContext.JobFreqTypeEntities.ToListAsync();

            return jobFreqTypes.Select(x => x.ToDataModel());
        }

        public Task<bool> SoftDelete(JobKey key)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StartJob(JobKey key)
        {
            var (user, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            var job = await dbContext.JobEntities
                // .Include(js => js.JobScheduleEntity)
                // .Include(r => r.JobFreqType)
                .FirstOrDefaultAsync(j => j.Id == key.Id);

            if (job == null) throw new Exception("Job not found.");

            // if (job.JobScheduleEntity == null) throw new Exception("Job schedule not found.");

            if (job.JobStatusType != JobStatusType.Queued)
                throw new Exception("Job must be in queued state to start.");

            job.JobStatusType = JobStatusType.Running;
            job.StatusLog = key.StatusLog ?? job.StatusLog;
            job.RecentScheduledRunDateTime = DateTime.Now;

            if (job.JobFreqType is null)
                throw new Exception("Job frequency type not found.");

            var nextRunDateTime = GenerateNextRunTime(job.JobFreqType.ScheduleType,
                job.NextScheduledRunDateTime);

            if (job.LastScheduledRunDateTime != null)
                if (nextRunDateTime >= job.LastScheduledRunDateTime)
                    throw new Exception("Job schedule has expired. Please generate a new one!");

            job.NextScheduledRunDateTime =
                GenerateNextRunTime(job.JobFreqType.ScheduleType);

            job.Updated = DateTime.Now;
            job.UpdatedUser = user;
            
            dbContext.Entry(job).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompleteJob(JobKey key)
        {
            var (user, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            var job = await dbContext.JobEntities
                // .Include(js => js.JobScheduleEntity)
                // .Include(r => r.JobFreqType)
                .FirstOrDefaultAsync(j => j.Id == key.Id);

            if (job == null) throw new Exception("Job not found.");

            // if (job.JobScheduleEntity == null) throw new Exception("Job schedule not found.");

            if (!new[] { JobStatusType.Queued, JobStatusType.Completed, JobStatusType.Cancelled }
                    .Contains(job.JobStatusType))
                throw new Exception("Job must be in running state to complete.");

            job.JobStatusType = JobStatusType.Completed;
            job.StatusLog = key.StatusLog ?? job.StatusLog;
            job.RecentScheduledRunDateTime = DateTime.Now;

            if (job.JobFreqType is null)
                throw new Exception("Job frequency type not found.");

            var nextRunDateTime = GenerateNextRunTime(job.JobFreqType.ScheduleType,
                job.NextScheduledRunDateTime);

            if (job.LastScheduledRunDateTime != null)
                if (nextRunDateTime >= job.LastScheduledRunDateTime)
                    throw new Exception("Job schedule has expired. Please generate a new one!");

            job.NextScheduledRunDateTime =
                GenerateNextRunTime(job.JobFreqType.ScheduleType);

            job.Updated = DateTime.Now;
            job.UpdatedUser = user;

            dbContext.Entry(job).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> QueueJob(JobKey key)
        {
            var (user, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            var job = await dbContext.JobEntities
                // .Include(js => js.JobScheduleEntity)
                // .Include(r => r.JobFreqType)
                .FirstOrDefaultAsync(j => j.Id == key.Id);

            if (job == null) throw new Exception("Job not found.");

            // if (job.JobScheduleEntity == null) throw new Exception("Job schedule not found.");

            if (!new[] { JobStatusType.Running, JobStatusType.Queued }
                    .Contains(job.JobStatusType))
                throw new Exception("Job must be in a Completed or Cancelled state");

            job.JobStatusType = JobStatusType.Queued;
            job.StatusLog = key.StatusLog ?? job.StatusLog;
            job.RecentScheduledRunDateTime = DateTime.Now;

            if (job.JobFreqType is null)
                throw new Exception("Job frequency type not found.");

            var nextRunDateTime = GenerateNextRunTime(job.JobFreqType.ScheduleType,
                job.NextScheduledRunDateTime);

            if (job.LastScheduledRunDateTime != null)
                if (nextRunDateTime >= job.LastScheduledRunDateTime)
                    throw new Exception("Job schedule has expired. Please generate a new one!");

            job.NextScheduledRunDateTime =
                GenerateNextRunTime(job.JobFreqType.ScheduleType);

            job.Updated = DateTime.Now;
            job.UpdatedUser = user;

            dbContext.Entry(job).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelJob(JobKey key)
        {
            var (user, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            var job = await dbContext.JobEntities
                // .Include(js => js.JobScheduleEntity)
                // .Include(r => r.JobFreqType)
                .FirstOrDefaultAsync(j => j.Id == key.Id);

            if (job == null) throw new Exception("Job not found.");

            // if (job.JobScheduleEntity == null) throw new Exception("Job schedule not found.");

            if (!new[] { JobStatusType.Cancelled, JobStatusType.Completed }
                    .Contains(job.JobStatusType))
                throw new Exception("Job must be in a Running or Queued state to fail.");

            job.JobStatusType = JobStatusType.Cancelled;
            job.StatusLog = key.StatusLog ?? job.StatusLog;
            job.RecentScheduledRunDateTime = DateTime.Now;

            if (job.JobFreqType is null)
                throw new Exception("Job frequency type not found.");

            var nextRunDateTime = GenerateNextRunTime(job.JobFreqType.ScheduleType,
                job.NextScheduledRunDateTime);

            if (job.LastScheduledRunDateTime != null)
                if (nextRunDateTime >= job.LastScheduledRunDateTime)
                    throw new Exception("Job schedule has expired. Please generate a new one!");

            job.NextScheduledRunDateTime =
                GenerateNextRunTime(job.JobFreqType.ScheduleType);

            job.Updated = DateTime.Now;
            job.UpdatedUser = user;

            // dbContext.Entry(job.JobScheduleEntity).State = EntityState.Modified;
            dbContext.Entry(job).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return true;
        }

        // We should manage the job schedule here
    }
}