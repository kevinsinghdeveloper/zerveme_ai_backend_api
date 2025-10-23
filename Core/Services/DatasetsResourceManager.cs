using System.Runtime.CompilerServices;
using System.Text.Json;
using zervemedata.Data.AuthData;
using zervemedata.Data.Entities.Keys;

namespace zervemedata.Core.Services
{
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data;
    using zervemedata.Core.Services.DataWarehouseManagers;
    using Microsoft.EntityFrameworkCore;
    using zervemedata.Data.Entities;
    using zervemedata.Core.Extensions;
    using System.Diagnostics;
    using zerveme.Utilities;
    using zervemedata.Data.DataModels.Data;
    using zervemedata.Data.DataModels.Querying;
    using zervemedata.Data.DataModels.Responses;
    using Newtonsoft.Json;
    using System.Security.Claims;
    using Google.Apis.Util;
    using zervemedata.Data.JsonDataModels;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;
    
    
    using IDatasetResourceManager = zervemedata.Core.Contracts.Abstractions.IDatasetResourceManager<
        zervemedata.Data.DataModels.CreateMeta.DatasetCreationMetaData,
        zervemedata.Data.Entities.Keys.DatasetKey,
        zervemedata.Data.DataModels.CreateMeta.DatasetUpdateMetaData,
        zervemedata.Data.DataModels.Data.DatasetData, zervemedata.Data.DataModels.Querying.DatasetDWHQueryingParameters,
        zervemedata.Data.DataModels.Responses.DatasetQueriedData,
        zervemedata.Data.DataModels.Responses.DatasetTemplateQueryData>;

    public class DatasetsResourceManager(
        ZervemedataDbContext dbContext,
        DatawarehouseHandlerService datawarehouseHandlerService,
        IHttpContextAccessor httpContextAccessor)
        : IDatasetResourceManager
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

        
        public async Task<DatasetData> Create(DatasetCreationMetaData creationMetaData)
        {
            var (_, _, organization) = await ValidateUserAndOrg(creationMetaData.CurrentUser);
            var report = await dbContext.ReportEntities
                .Where(r => r.Id == creationMetaData.ReportId)
                .FirstOrDefaultAsync();

            if (report == null)
            {
                throw new Exception("The Report could not be found!");
            }

            // check dwh
            var dwh = await dbContext.DwhEntities
                .Where(d => d.Id == creationMetaData.DWHId)
                .FirstOrDefaultAsync();
            if (dwh == null)
            {
                throw new Exception("The DWH could not be found!");
            }

            //JsonConvert.SerializeObject(creationMetaData.DomainData)
            var datasetEntity = new DatasetEntity()
            {
                Id = Guid.NewGuid(),
                Name = creationMetaData.Name,
                DomainData = creationMetaData.DomainData, // TODO test this I swap the entity object!!!
                VizResponseData = creationMetaData.VizResponseData,
                Created = DateTime.Now,
                Dwh = dwh,
                ReportEntity = report,
                OrganizationEntity = organization
            };

            dbContext.Add(datasetEntity);

            await dbContext.SaveChangesAsync();

            return datasetEntity.ToDataModel();
        }

        public async Task<DatasetData> Update(DatasetUpdateMetaData updateMetaData)
        {
            var entity = await dbContext.DatasetEntities.Where(d => d.Id == updateMetaData.Id)
                .FirstOrDefaultAsync();
            
            if (entity == null)
            {
                throw new Exception("The Dataset could not be found!");
            }

            //JsonConvert.SerializeObject(datasetUpdateMetaData.DomainData);
            entity.Name = updateMetaData.Name ?? entity.Name;
            entity.DomainData = updateMetaData.DomainData ?? entity.DomainData;
            entity.Updated = DateTime.Now;
            entity.DWHId = updateMetaData.DWHId ?? entity.DWHId;
            entity.VizResponseData = updateMetaData.VizResponseData ?? entity.VizResponseData;
            entity.DataRefreshedDate = updateMetaData.DataRefreshedDate ?? entity.DataRefreshedDate;

            // get report
            var report = await dbContext.ReportEntities
                .Where(r => r.Id == updateMetaData.ReportId)
                .FirstOrDefaultAsync();

            if (report != null)
            {
                entity.ReportEntity = report;
            }
            
            dbContext.Update(entity);

            await dbContext.SaveChangesAsync();
            
            return entity.ToDataModel();
        }

        public async Task<DatasetData> Get(DatasetKey key)
        {
            if (key.Id == null)
            {
                throw new Exception("The Dataset Id cannot be null!");
            }

            var entity = await dbContext.DatasetEntities.Where(d => d.Id == key.Id)
                .Where(d => d.Deleted == null)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception("The Dataset could not be found!");
            }

            return entity.ToDataModel();
        }

        public async Task<DatasetData> GetDatasetWithEtlConfig(DatasetKey key)
        {
            if (key.Id == null)
            {
                throw new Exception("The Dataset Id cannot be null!");
            }

            var entity = await dbContext.DatasetEntities.Where(d => d.Id == key.Id)
                // .Include(d => d.DatasetEtlConfigEntity)
                .Where(d => d.Deleted == null)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception("The Dataset could not be found!");
            }

            return entity.ToDataModel();
        }

        public async Task<IEnumerable<DatasetData>> GetAll(DatasetKey key)
        {
            var (_, _, organization) = await ValidateUserAndOrg(key.CurrentUser);
            
            var entities = await dbContext.DatasetEntities
                // .Include(d => d.DatasetEtlConfigEntity)
                .Where(d => d.Deleted == null && d.OrganizationId == organization.Id)
                .ToListAsync();

            if (!entities.Any())
            {
                throw new Exception("No Datasets found!");
            }

            return entities.Select(e => e.ToDataModel()).ToList();
        }

        public async Task<DatasetQueriedData> GetData(DatasetDWHQueryingParameters queryingParameters,
            string currentUser)
        {
            var datasetQueryingParameters = queryingParameters;

            var (user, _, organization) = await ValidateUserAndOrg(currentUser);

            // TODO either provide a datasetid or a reportid 

            DatasetEntity dataset;

            if (queryingParameters.ReportId != null)
            {
                dataset = await dbContext.DatasetEntities.Where(d => d.Deleted == null
                                                                     && d.ReportId == datasetQueryingParameters
                                                                         .ReportId)
                    .Include(d => d.Dwh)
                    .FirstOrDefaultAsync() ?? throw new Exception("Dataset not found!");
            }
            else if (queryingParameters.DatasetId != null)
            {
                dataset = await dbContext.DatasetEntities.Where(d => d.Deleted == null
                                                                     && d.Id == datasetQueryingParameters
                                                                         .DatasetId)
                    .Include(d => d.Dwh)
                    .FirstOrDefaultAsync() ?? throw new Exception("Dataset not found!");
            }
            else
            {
                throw new Exception("Either a DatasetId or ReportId must be provided!");
            }

            dataset.Dwh.ThrowIfNull("DWH not found!");
            
            //create a service to help us parse the dataset domain data
            var domainData = dataset.DomainData;
            
            var dwService = datawarehouseHandlerService.GetDatawarehouseService(dataset.Dwh);

            if (dwService is null)
            {
                throw new Exception("Failed to create DWH service");
            }
            
            var kpiColumns = datasetQueryingParameters.KPIs?.ToList() ?? new List<string>();
            var attrColumns = datasetQueryingParameters.Attributes?.ToList() ?? new List<string>();

            var tableName = domainData.TableName; // New property

            if (tableName is null or "")
            {
                throw new Exception("Table name is empty");
            }

            Dictionary<string, string> aggMethods = domainData.KpiCols?
                .ToDictionary(
                    col => col.ColumnName,
                    col => col.AggregationType.ToString()
                ) ?? new();

            // TODO somehow add a agg map here for the metrics in use AggregationType
            var query = dwService.ConstructSelectQuery(
                tableName,
                dataset.Dwh?.PrimarySchema ?? "",
                kpiColumns.ToList(),
                attrColumns.ToList(),
                aggMethods,
                datasetQueryingParameters.Filters,
                datasetQueryingParameters.RowLimit ?? 50
            );
            if (query == null)
            {
                throw new Exception("Failed to generate query");
            }
            
            var stopwatch = Stopwatch.StartNew();

            object? results = null;
            List<string>? error = new List<string>();

            var queriedResult = dwService?.SelectQuery(query);

            if (queriedResult?.error != null)
            {
                error.Add($"Query error: {queriedResult.error}");
            }
            
            try
            {
                var result = queriedResult?.result;
                results = result != null ? JsonDocument.Parse(result) : null;
            }
            catch (Exception ex)
            {
                error.Add($"Failed to parse query response: {ex}");
            }

            stopwatch.Stop();

            var queriedData = new DatasetQueriedData()
            {
                Id = dataset.Id,
                QueryRunTimeElapsedSecs = Convert.ToInt32(stopwatch.Elapsed.TotalSeconds),
                RowLimit = datasetQueryingParameters.RowLimit ?? 50,
                RowsReturned = queriedResult?.rowsReturned ?? 0,
                Data = results ?? null,
                Error = error
            };
            
            var trackerRecord = new DatasetRunQueryTrackerEntity() // TODO add elapsed time here with start and end
            {
                Id = Guid.NewGuid(),
                DatasetId = dataset.Id,
                DwhId = dataset.DWHId ?? Guid.Empty,
                Query = query,
                Created = DateTime.Now,
                CreatedUser = user,
                QueryRunTimeElapsedSecs = Convert.ToInt32(stopwatch.Elapsed.TotalSeconds),
                RowLimit = datasetQueryingParameters.RowLimit ?? 50,
                RowsReturned = queriedResult?.rowsReturned ?? 0
            };

            await dbContext.AddAsync(trackerRecord);

            await dbContext.SaveChangesAsync();

            return queriedData;
        }

        public async Task<DatasetTemplateQueryData> GetVizData(DatasetDWHQueryingParameters queryingParameters,
            string currentUser)
        {
            var datasetQueryingParameters = queryingParameters;

            var (user, _, organization) = await ValidateUserAndOrg(currentUser);

            DatasetEntity dataset;

            if (queryingParameters.ReportId != null)
            {
                dataset = await dbContext.DatasetEntities.Include(d => d.ReportEntity).Where(d => d.Deleted == null
                                                                     && d.ReportId == datasetQueryingParameters
                                                                         .ReportId)
                    .Include(d => d.Dwh)
                    .FirstOrDefaultAsync() ?? throw new Exception("Dataset not found!");
            }
            else if (queryingParameters.DatasetId != null)
            {
                dataset = await dbContext.DatasetEntities.Include(d => d.ReportEntity).Where(d => d.Deleted == null
                                                                     && d.Id == datasetQueryingParameters
                                                                         .DatasetId)
                    .Include(d => d.Dwh)
                    .FirstOrDefaultAsync() ?? throw new Exception("Dataset not found!");
            }
            else
            {
                throw new Exception("Either a DatasetId or ReportId must be provided!");
            }

            dataset.Dwh.ThrowIfNull("DWH not found!");

            if (dataset.ReportEntity == null)
            {
                throw new Exception("The Dataset does not have a Report associated with it!");
            }

            // get report confiuration
            var reportConfiguration = await dbContext.ReportConfigurationEntities
                .Where(rc => rc.ReportTypeId == dataset.ReportEntity.ReportTypeId)
                .FirstOrDefaultAsync();

            if (reportConfiguration == null)
            {
                throw new Exception("Report configuration not found for the dataset's report type.");
            }

            if (reportConfiguration.VizTemplate == null)
            {
                throw new Exception("Report configuration does not have a visualization template.");
            }

            if (dataset.VizResponseData == null)
            {
                throw new Exception("Dataset does not have visualization response data.");
            }
            
            var queriedData = new DatasetTemplateQueryData()
            {
                Id = dataset.Id,
                VizResponseData = dataset.VizResponseData,
                VizResponseTemplate = reportConfiguration.VizTemplate
            };
            
            await dbContext.SaveChangesAsync();
            return queriedData;
        }

        public async Task<IEnumerable<DatasetData>> GetAllDatasetNames(DatasetKey key)
        {
            var (_, _, organization) = await ValidateUserAndOrg(key.CurrentUser);
            
            var entities = await dbContext.DatasetEntities
                // .Include(d => d.DatasetEtlConfigEntity)
                .Where(d => d.Deleted == null && d.OrganizationId == organization.Id)
                .ToListAsync();

            if (!entities.Any())
            {
                throw new Exception("No Datasets found!");
            }

            return entities.Select(e => new DatasetData
            {
                Id = e.Id,
                Name = e.Name
            }).ToList();
        }

        public async Task<DatasetData> GetDomainOptions(DatasetKey key)
        {
            if (key.Id == null)
            {
                throw new Exception("The Dataset Id cannot be null!");
            }

            var entity = await dbContext.DatasetEntities.Where(d => d.Id == key.Id)
                .Where(d => d.Deleted == null)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new Exception("The Dataset could not be found!");
            }

            return entity.ToDataModel(onlyDomainData: true);
        }
    }
}

/*
 * {
  "datasetname": "TestData",
  "tablename": "fact_test",
  "kpi_cols": ["dollars", "volume"],
  "attr_cols": ["category", "country"],
  "all_cols": ["dollars", "volume", "category", "country"],
  "period_column": "date_created",
  "updated_date_time": "2023-03-16 5:00",
  "num_rows": 5003,
  "connector_identifier": "21312AAA",
  "custom_sql": ""
}
*/