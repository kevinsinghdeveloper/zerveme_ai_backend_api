namespace zervemedata.Core.Extensions
{
    using zervemedata.Data.DataModels.Data;
    using zervemedata.Data.Entities;
    using zervemedata.Data.AuthData;
    using Microsoft.AspNetCore.Identity;
    using Newtonsoft.Json;
    using zervemedata.Data.JsonDataModels;
    public static class ModelConversionExtensions
    {
        internal static DWHData ToDataModel(this DWHEntity entityModel)
        {
            return new DWHData()
            {
                Name = entityModel.Name,
                Schema = entityModel.PrimarySchema,
                ConnectionString = entityModel.ConnectionString,
                dwhName = entityModel.Name ?? "",
                Id = entityModel.Id,
            };
        }


        internal static DatasetData ToDataModel(this DatasetEntity entityModel, bool onlyDomainData = false)
        {
            var domainData = entityModel.DomainData;
            
            if (onlyDomainData)
            {
                return new DatasetData()
                {
                    Id = entityModel.Id,
                    Name = entityModel.Name,
                    DomainData = domainData
                };
            }
            
            return new DatasetData()
            {
                Id = entityModel.Id,
                Name = entityModel.Name,
                DomainData = domainData,
                VizResponseData = entityModel.VizResponseData,
                Updated = entityModel.Updated,
                Created = entityModel.Created,
                Deleted = entityModel.Deleted,
                DWHId = entityModel.DWHId,
                DataRefreshedDate = entityModel.DataRefreshedDate
            };
        }

        internal static UserInfo ToDataModel(this ApplicationUser entityModel, OrganizationEntity? organizationEntity)
        {
            return new UserInfo
            {
                Id = Guid.Parse(entityModel.Id),
                FirstName = entityModel.FirstName,
                LastName = entityModel.LastName,
                Email = entityModel.Email ?? "",
                UserName = entityModel.UserName ?? "",
                Address1 = entityModel.ContactEntity?.Address1 ?? null,
                Address2 = entityModel.ContactEntity?.Address2 ?? null,
                Address3 = entityModel.ContactEntity?.Address3 ?? null,
                City = entityModel.ContactEntity?.City ?? null,
                State = entityModel.ContactEntity?.State ?? null,
                Country = entityModel.ContactEntity?.Country ?? null,
                ZipCode = entityModel.ContactEntity?.Address2 ?? null,
                PrimaryPhone = entityModel.ContactEntity?.PrimaryPhone ?? null,
                SecondaryPhone = entityModel.ContactEntity?.SecondaryPhone ?? null,
                NotificationEmail = entityModel.ContactEntity?.NotificationEmail ?? null,
                OrganizationId = organizationEntity?.Id ?? Guid.Empty,
            };
        }

        internal static OrganizationsInfo ToDataModel(this OrganizationEntity organizationEntity)
        {
            return new OrganizationsInfo()
            {
                Id = organizationEntity.Id,
                Name = organizationEntity.Name,
            };
        }
        
        internal static RoleInfo ToDataModel(this IdentityRole entityModel)
        {
            return new RoleInfo()
            {
                RoleId = entityModel.Id,
                RoleName = entityModel.Name ?? ""
            };
        }

        internal static ProjectInfo ToDataModel(this ProjectEntity entityModel)
        {
            return new ProjectInfo()
            {
                Id = entityModel.Id,
                Name = entityModel.Name,
                Description = entityModel.Description,
                Reports = entityModel.ReportEntities.Select(r => r.ToDataModel()),
            };
        }

        internal static ReportInfo ToDataModel(this ReportEntity entityModel,
            ReportConfigurationEntity? reportConfigurationEntity = null)
        {
            var model = entityModel.ModelEntity == null
                ? null
                : new
                {
                    Id = entityModel.ModelEntity.Id,
                    Name = entityModel.ModelEntity.Name ?? ""
                };
            return new ReportInfo()
            {
                Id = entityModel.Id,
                Name = entityModel.Name,
                Description = entityModel.Description,
                ProjectId = entityModel.ProjectId,
                ReportType = entityModel.ReportTypeEntity.ToDataModel(),
                Job = entityModel.JobEntity?.ToDataModel(),
                ReportConfigurationId = reportConfigurationEntity?.Id ?? Guid.Empty,
                DatasetConfig = entityModel.ReportDatasetEntity?.DatasetConfig,
                Model = model,
                DatasetData = entityModel.DatasetEntity?.ToDataModel(),
            };
        }

        internal static JobFreqTypeInfo ToDataModel(this JobFreqTypeEntity entityModel)
        {
            return new JobFreqTypeInfo()
            {
                Id = entityModel.Id,
                Name = entityModel.Name
            };
        }

        internal static ReportTypeInfo ToDataModel(this ReportTypeEntity entityModel)
        {
            return new ReportTypeInfo()
            {
                Id = entityModel.Id,
                Name = entityModel.Name,
                ReportConfig = entityModel.ReportConfigurationEntity?.ReportConfig,
            };
        }


        internal static JobInfo ToDataModel(this JobEntity entityModel)
        {
            return new JobInfo()
            {
                Id = entityModel.Id,
                JobFreqType = entityModel.JobFreqType?.ToDataModel(),
                UpdatedUserId = entityModel.UpdatedUserId,
                CreatedUserId = entityModel.CreatedUserId,
                Updated = entityModel.Updated,
                Created = entityModel.Created,
                Deleted = entityModel.Deleted,
                LastRunDate = entityModel.RecentScheduledRunDateTime,
                JobStatusType = entityModel.JobStatusType
            };
        }

        internal static ReportConfigurationInfo ToDataModel(this ReportConfigurationEntity entityModel)
        {
            return new ReportConfigurationInfo()
            {
                Id = entityModel.Id,
                ReportConfig = entityModel.ReportConfig,
                VizTemplate = entityModel.VizTemplate,
                ReportTypeId = entityModel.ReportTypeId
            };
        }

        internal static ReportDatasetInfo ToDataModel(this ReportDatasetEntity entityModel)
        {
            return new ReportDatasetInfo()
            {
                Id = entityModel.Id,
                ReportInfo = entityModel.ReportEntity.ToDataModel(),
                DatasetConfig = entityModel.DatasetConfig
            };
        }

        internal static ModelsInfo ToDataModel(this ModelEntity entityModel)
        {
            return new ModelsInfo()
            {
                Id = entityModel.Id,
                Name = entityModel.Name,
                Description = entityModel.Description,
                ModelConfig = entityModel.ModelConfig,
                ModelTypeId = entityModel.ModelTypeEntity.Id,
                ModelType = entityModel.ModelTypeEntity.Name
            };
        }

        internal static ModelTypeInfo ToDataModel(this ModelTypeEntity entityModel)
        {
            return new ModelTypeInfo()
            {
                Id = entityModel.Id,
                Name = entityModel.Name,
                Description = entityModel.Description
            };
        }
    }
}