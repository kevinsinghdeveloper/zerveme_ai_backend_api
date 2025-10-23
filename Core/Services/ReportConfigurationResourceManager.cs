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
    using IReportConfigurationResourceManager =
        zervemedata.Core.Contracts.Abstractions.IReportConfigurationResourceManager<
            zervemedata.Data.DataModels.Data.ReportConfigurationInfo,
            zervemedata.Data.DataModels.Data.ReportTypeInfo,
            zervemedata.Data.Entities.Keys.ReportConfigurationKey,
            zervemedata.Data.Entities.Keys.ReportKey,
            zervemedata.Data.DataModels.CreateMeta.ReportConfigurationCreationMetaData,
            zervemedata.Data.DataModels.CreateMeta.ReportTypeCreationMetaData,
            zervemedata.Data.DataModels.UpdateMeta.ReportConfigurationUpdateMetaData>;


    /*
     * Creation of configurations used for populating UI for reports -> data is used for dataset creation
     */
    public class ReportConfigurationResourceManager(
        ZervemedataDbContext dbContext) : IReportConfigurationResourceManager
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

        public async Task<ReportConfigurationInfo> Create(ReportConfigurationCreationMetaData creationMetaData)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == creationMetaData.CurrentUser);
            if (user == null) throw new Exception("User not found.");

            var reportType = await dbContext.ReportTypeEntities.FindAsync(creationMetaData.ReportTypeId);

            if (reportType == null)
            {
                throw new KeyNotFoundException(
                    $"The report type with id {creationMetaData.ReportTypeId} does not exist.");
            }

            // make sure no existing configuration exists -- otherwise they will need to updat
            var reportConfiguration = await dbContext.ReportConfigurationEntities
                .FirstOrDefaultAsync(r => r.ReportTypeId == creationMetaData.ReportTypeId);

            if (reportConfiguration != null)
            {
                throw new Exception($"The report type with id {creationMetaData.ReportTypeId} already exists.");
            }

            var newReportConfiguration = new ReportConfigurationEntity()
            {
                ReportType = reportType,
                ReportConfig = creationMetaData.ReportConfig,
                VizTemplate = creationMetaData.VizTemplate,
                CreatedUser = user,
                Created = DateTime.Now
            };

            await dbContext.ReportConfigurationEntities.AddAsync(newReportConfiguration);
            await dbContext.SaveChangesAsync();

            return newReportConfiguration.ToDataModel();
        }

        public async Task<ReportTypeInfo> CreateReportType(ReportTypeCreationMetaData creationMetaData)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == creationMetaData.CurrentUser);
            if (user == null) throw new Exception("User not found.");

            var reportType = await dbContext.ReportTypeEntities
                .FirstOrDefaultAsync(r => r.Name == creationMetaData.Name && r.Deleted == null);

            if (reportType != null)
            {
                throw new Exception($"The report type with name {creationMetaData.Name} already exists.");
            }

            var newReportType = new ReportTypeEntity()
            {
                Name = creationMetaData.Name,
                Description = creationMetaData.Description ?? "",
                Created = DateTime.Now
            };

            var newReportConfiguration = new ReportConfigurationEntity()
            {
                ReportType = newReportType,
                ReportConfig = creationMetaData.ReportConfig,
                VizTemplate = creationMetaData.VizTemplate,
                CreatedUser = user,
                Created = DateTime.Now
            };

            await dbContext.ReportTypeEntities.AddAsync(newReportType);
            await dbContext.ReportConfigurationEntities.AddAsync(newReportConfiguration);
            await dbContext.SaveChangesAsync();
            return newReportType.ToDataModel();
        }

        public async Task<ReportConfigurationInfo> Update(ReportConfigurationUpdateMetaData updateMetaData)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == updateMetaData.CurrentUser);
            if (user == null) throw new Exception("User not found.");

            var reportConfiguration = await dbContext.ReportConfigurationEntities
                .FirstOrDefaultAsync(r => r.Id == updateMetaData.Id);

            if (reportConfiguration == null)
            {
                throw new Exception("Report configuration not found.");
            }

            if (updateMetaData.ReportTypeId != null)
            {
                var reportType = await dbContext.ReportTypeEntities
                    .FirstOrDefaultAsync(r => r.Id == updateMetaData.ReportTypeId && r.Deleted == null);

                if (reportType == null)
                {
                    throw new Exception("Report type not found.");
                }

                reportConfiguration.ReportType = reportType;
            }

            reportConfiguration.ReportConfig = updateMetaData.ReportConfig ?? reportConfiguration.ReportConfig;
            reportConfiguration.VizTemplate = updateMetaData.VizTemplate ?? reportConfiguration.VizTemplate;
            reportConfiguration.UpdatedUser = user;
            reportConfiguration.Updated = DateTime.Now;

            dbContext.ReportConfigurationEntities.Update(reportConfiguration);
            await dbContext.SaveChangesAsync();

            return reportConfiguration.ToDataModel();
        }

        public async Task<bool> SoftDelete(ReportConfigurationKey key)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == key.CurrentUser);
            if (user == null) throw new Exception("User not found.");

            var reportConfiguration = await dbContext.ReportConfigurationEntities
                .FirstOrDefaultAsync(r => r.Id == key.Id && r.Deleted == null);

            if (reportConfiguration == null)
            {
                throw new Exception("Report configuration not found.");
            }

            reportConfiguration.Deleted = DateTime.Now;
            reportConfiguration.UpdatedUser = user;
            dbContext.ReportConfigurationEntities.Update(reportConfiguration);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ReportConfigurationInfo>> GetAll(ReportConfigurationKey key)
        {
            // when grabbing, ensure user is part of an organization
            await ValidateUserAndOrg(key.CurrentUser);

            // get all configurations
            return await dbContext.ReportConfigurationEntities
                .Include(r => r.ReportType)
                .Where(r => r.Deleted == null)
                .Select(r => r.ToDataModel())
                .ToListAsync();
        }

        public async Task<ReportConfigurationInfo?> Get(ReportConfigurationKey key)
        {
            // when grabbing, ensure user is part of an organization
            await ValidateUserAndOrg(key.CurrentUser);

            // get configurations
            var reportConfiguration = await dbContext.ReportConfigurationEntities
                .Include(r => r.ReportType)
                .FirstOrDefaultAsync(r => r.Id == key.Id && r.Deleted == null);

            return reportConfiguration?.ToDataModel();
        }

        public async Task<IEnumerable<ReportTypeInfo>> GetAllReportTypes(ReportKey key)
        {
            var (_, _, _) = await ValidateUserAndOrg(key.CurrentUser);

            return await dbContext.ReportTypeEntities
                .Include(r => r.ReportConfigurationEntity)
                .Select(rt => rt.ToDataModel())
                .ToListAsync();
        }

    }
}