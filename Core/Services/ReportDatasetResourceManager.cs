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
    using IReportDatasetResourceManager = zervemedata.Core.Contracts.Abstractions.IReportDatasetResourceManager<
        zervemedata.Data.DataModels.Data.ReportDatasetInfo,
        zervemedata.Data.Entities.Keys.ReportDatasetKey,
        zervemedata.Data.DataModels.CreateMeta.ReportDatasetCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ReportDatasetUpdateMetaData>;

    public class ReportDatasetResourceManager(
        ZervemedataDbContext dbContext) : IReportDatasetResourceManager
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

        public async Task<ReportDatasetInfo> Create(ReportDatasetCreationMetaData creationMetaData)
        {
            var (user, _, _) = await ValidateUserAndOrg(creationMetaData.CurrentUser);

            var report = await dbContext.ReportEntities
                             .FirstOrDefaultAsync(r => r.Id == creationMetaData.ReportId)
                         ?? throw new Exception("Report not found.");

            var reportConfig = await dbContext.ReportConfigurationEntities
                                   .FirstOrDefaultAsync(r => r.Id == creationMetaData.ReportConfigurationId)
                               ?? throw new Exception("Report configuration not found.");

            var reportDataset = new ReportDatasetEntity()
            {
                ReportConfigurationEntity = reportConfig,
                ReportEntity = report,
                DatasetConfig = creationMetaData.DatasetConfig,
                CreatedUser = user,
                Created = DateTime.UtcNow,
                UpdatedUser = user,
                Updated = DateTime.UtcNow
            };

            await dbContext.ReportDatasetEntities.AddAsync(reportDataset);
            await dbContext.SaveChangesAsync();

            return reportDataset.ToDataModel();
        }

        public async Task<ReportDatasetInfo> Update(ReportDatasetUpdateMetaData updateMetaData)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ReportTypeInfo>> GetAllReportTypes(ReportDatasetKey key)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SoftDelete(ReportDatasetKey key)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ReportDatasetInfo>> GetAll(ReportDatasetKey key)
        {
            throw new NotImplementedException();
        }

        public async Task<ReportDatasetInfo?> Get(ReportDatasetKey key)
        {
            throw new NotImplementedException();
        }
    }
}