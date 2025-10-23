using zervemedata.Data.DataModels.Querying;
using zervemedata.Data.DataModels.Responses;
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
    using IReportResourceManager = zervemedata.Core.Contracts.Abstractions.IReportResourceManager<
        zervemedata.Data.DataModels.Data.ReportInfo,
        zervemedata.Data.DataModels.Data.ReportTypeInfo,
        zervemedata.Data.DataModels.Data.DatasetData,
        zervemedata.Data.Entities.Keys.ReportKey,
        zervemedata.Data.DataModels.CreateMeta.ReportCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ReportUpdateMetaData>;

    public class ReportResourceManager(
        ZervemedataDbContext dbContext) : IReportResourceManager
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

        public async Task<ReportInfo> Create(ReportCreationMetaData creationMetaData)
        {
            var (user, orgUser, _) = await ValidateUserAndOrg(creationMetaData.CurrentUser);

            var project = await dbContext.ProjectEntities.FirstOrDefaultAsync(p => p.Id == creationMetaData.ProjectId)
                          ?? throw new Exception("Project not found.");

            var reportType =
                await dbContext.ReportTypeEntities
                    .Include(r => r.ReportConfigurationEntity)
                    .FirstOrDefaultAsync(rt => rt.Id == creationMetaData.ReportTypeId)
                ?? throw new Exception("Report type not found.");

            var job = await dbContext.JobEntities.FirstOrDefaultAsync(j => j.Id == creationMetaData.JobId)
                      ?? throw new Exception("Job not found.");

            var model = await dbContext.ModelEntities
                            .FirstOrDefaultAsync(m => m.Id == creationMetaData.ModelId)
                        ?? throw new Exception("Model not found.");

            var report = new ReportEntity()
            {
                Name = creationMetaData.Name,
                Description = creationMetaData.Description,
                ProjectEntity = project,
                ReportTypeEntity = reportType,
                CreatedUser = user,
                Created = DateTime.Now,
                JobEntity = job,
                ModelEntity = model
            };

            await dbContext.ReportEntities.AddAsync(report);
            await dbContext.SaveChangesAsync();

            return report.ToDataModel(reportType.ReportConfigurationEntity);
        }

        public async Task<ReportInfo> Update(ReportUpdateMetaData updateMetaData)
        {
            var (user, orgUser, _) = await ValidateUserAndOrg(updateMetaData.CurrentUser);
            var report = await dbContext.ReportEntities
                             .Include(r => r.ModelEntity)
                             .FirstOrDefaultAsync(r => r.Id == updateMetaData.Id)
                         ?? throw new Exception("Report not found.");

            var project = await dbContext.ProjectEntities.FirstOrDefaultAsync(p => p.Id == updateMetaData.ProjectId)
                          ?? throw new Exception("Project not found.");

            var reportType =
                await dbContext.ReportTypeEntities.FirstOrDefaultAsync(rt => rt.Id == updateMetaData.ReportTypeId)
                ?? throw new Exception("Report type not found.");

            if (report.ReportDatasetEntity == null)
            {
                throw new Exception("Report dataset entity not found.");
            }

            if (updateMetaData.DatasetConfig != null)
            {
                report.ReportDatasetEntity.DatasetConfig = updateMetaData.DatasetConfig;
                report.ReportDatasetEntity.UpdatedUser = user;
                report.ReportDatasetEntity.Updated = DateTime.Now;
            }

            if (updateMetaData.ModelId != null)
            {
                var model = await dbContext.ModelEntities
                                .FirstOrDefaultAsync(m => m.Id == updateMetaData.ModelId)
                            ?? throw new Exception("Model not found.");
                report.ModelEntity = model;
            }
            

            report.Name = updateMetaData.Name;
            report.Description = updateMetaData.Description;
            report.ProjectEntity = project;
            report.ReportTypeEntity = reportType;
            report.UpdatedUser = user;
            report.Updated = DateTime.Now;

            if (report.JobEntity != null)
                report.JobEntity.JobFreqTypeId = updateMetaData.JobFreqTypeId ?? report.JobEntity.JobFreqTypeId;

            dbContext.ReportEntities.Update(report);
            dbContext.ReportDatasetEntities.Update(report.ReportDatasetEntity);
            await dbContext.SaveChangesAsync();

            return report.ToDataModel();
        }

        public async Task<IEnumerable<ReportTypeInfo>> GetAllReportTypes(ReportKey key)
        {
            var (_, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            return await dbContext.ReportTypeEntities
                .Include(r => r.ReportConfigurationEntity)
                .Select(rt => rt.ToDataModel())
                .ToListAsync();
        }

        public async Task<DatasetData> GetReportDomainOptions(ReportKey key)
        {
            var (_, _, _) = await ValidateUserAndOrg(key.CurrentUser);
            
            var dataset = await dbContext.DatasetEntities.FirstOrDefaultAsync(d => d.ReportId == key.Id);

            if (dataset == null) throw new Exception("Dataset not found.");

            return dataset.ToDataModel();
        }

        public async Task<bool> SoftDelete(ReportKey key)
        {
            var (user, orgUser, _) = await ValidateUserAndOrg(key.CurrentUser);
            var report = await dbContext.ReportEntities
                             .FirstOrDefaultAsync(r =>
                                 r.Id == key.Id && r.ProjectEntity.OrganizationId == orgUser.OrganizationId)
                         ?? throw new Exception("Report not found.");

            var job = report.JobEntity;

            if (job != null)
            {
                // var jobSchedule = job.JobScheduleEntity;
                //
                // if (jobSchedule != null)
                // {
                //     jobSchedule.Deleted = DateTime.Now;
                //     jobSchedule.UpdatedUser = user;
                //     dbContext.JobScheduleEntities.Update(jobSchedule);
                // }

                job.Deleted = DateTime.Now;
                job.UpdatedUser = user;
                dbContext.JobEntities.Update(job);
            }
            
            report.Deleted = DateTime.Now;
            report.UpdatedUser = user;

            dbContext.ReportEntities.Update(report);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ReportInfo>> GetAll(ReportKey key)
        {
            var (_, orgUser, _) = await ValidateUserAndOrg(key.CurrentUser);

            return await dbContext.ReportEntities
                .Include(r => r.ModelEntity)
                .Where(r => r.ProjectEntity.OrganizationId == orgUser.OrganizationId
                            && r.Deleted == null) // Only include non-deleted reports
                .Select(r => r.ToDataModel(null))
                .ToListAsync();
        }

        public async Task<ReportInfo?> Get(ReportKey key)
        {
            var (_, orgUser, _) = await ValidateUserAndOrg(key.CurrentUser);
            var report = await dbContext.ReportEntities
                .Include(r => r.ModelEntity)
                .FirstOrDefaultAsync(r => r.Id == key.Id
                                          && r.ProjectEntity.OrganizationId == orgUser.OrganizationId
                                          && r.Deleted == null); // Only include non-deleted report
            return report?.ToDataModel();
        }

        public async Task<IEnumerable<ReportInfo>> GetAllReportsFromProject(ReportKey key)
        {
            var (_, orgUser, _) = await ValidateUserAndOrg(key.CurrentUser);

            // get project
            var project = await dbContext.ProjectEntities
                .FirstOrDefaultAsync(p => p.Id == key.ProjectId
                                          && p.OrganizationId == orgUser.OrganizationId
                                          && p.Deleted == null); // Only consider non-deleted projects

            if (project == null)
            {
                throw new Exception("Project not found.");
            }

            // TODO get reportConfiguration Entity and Data configuration NEXT!
            
            // get reports
            var reports = await dbContext.ReportEntities
                .Include(r => r.ModelEntity)
                .Where(r => r.ProjectEntity.OrganizationId == orgUser.OrganizationId
                            && r.ProjectEntity.Id == project.Id
                            && r.Deleted == null) // Only include non-deleted reports
                .Select(r => r.ToDataModel(null))
                .ToListAsync();

            return reports;

            // get report configurations for each
            // var reportConfigurations = await dbContext.ReportConfigurationEntities
            //     .Where(rc => reports.Select(r => r.Id).Contains(rc.ReportEntityId))
            //     .ToListAsync();
        }

    }
}