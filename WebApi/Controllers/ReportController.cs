using zervemedata.Core.Contracts.Abstractions;
using zervemedata.Data.DataModels.Querying;
using zervemedata.Data.DataModels.UpdateMeta;
using zervemedata.Data.Entities.Keys;

namespace zervemedata.api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data.DataModels.Requests;
    using zervemedata.Data.Enumerations;
    using Microsoft.AspNetCore.Authorization;
    
    using IReportResourceManager = zervemedata.Core.Contracts.Abstractions.IReportResourceManager<
        zervemedata.Data.DataModels.Data.ReportInfo,
        zervemedata.Data.DataModels.Data.ReportTypeInfo,
        zervemedata.Data.DataModels.Data.DatasetData,
        zervemedata.Data.Entities.Keys.ReportKey,
        zervemedata.Data.DataModels.CreateMeta.ReportCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ReportUpdateMetaData>;
    
    using IJobResourceManager = zervemedata.Core.Contracts.Abstractions.IJobResourceManager<
        zervemedata.Data.DataModels.Data.JobInfo,
        zervemedata.Data.DataModels.Data.JobFreqTypeInfo,
        zervemedata.Data.Entities.Keys.JobKey,
        zervemedata.Data.DataModels.CreateMeta.JobCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.JobUpdateMetaData,
        zervemedata.Data.DataModels.UpdateMeta.JobScheduleUpdateMetaData
    >;
    using IReportDatasetResourceManager = zervemedata.Core.Contracts.Abstractions.IReportDatasetResourceManager<
        zervemedata.Data.DataModels.Data.ReportDatasetInfo,
        zervemedata.Data.Entities.Keys.ReportDatasetKey,
        zervemedata.Data.DataModels.CreateMeta.ReportDatasetCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ReportDatasetUpdateMetaData>;
    using IDatasetResourceManager = zervemedata.Core.Contracts.Abstractions.IDatasetResourceManager<
        zervemedata.Data.DataModels.CreateMeta.DatasetCreationMetaData,
        zervemedata.Data.Entities.Keys.DatasetKey,
        zervemedata.Data.DataModels.CreateMeta.DatasetUpdateMetaData,
        zervemedata.Data.DataModels.Data.DatasetData, zervemedata.Data.DataModels.Querying.DatasetDWHQueryingParameters,
        zervemedata.Data.DataModels.Responses.DatasetQueriedData,
        zervemedata.Data.DataModels.Responses.DatasetTemplateQueryData>;
    
    [Authorize(Policy = RoleConstants.User)]
    [Route("api/report")]
    [ApiController]
    public class ReportController(
        IReportResourceManager reportManager,
        IJobResourceManager jobManager,
        IReportDatasetResourceManager reportDatasetManager,
        IDatasetResourceManager datasetManager) : ControllerBase
    {
        [HttpPost("create", Name = nameof(ReportController.CreateReport))]
        public async Task<IActionResult> CreateReport(CreateReportRequest reportRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var jobCreationMeta = new JobCreationMetaData()
            {
                JobFreqTypeId = reportRequest.JobFreqTypeId,
                CurrentUser = user
            };

            var job = await jobManager.Create(jobCreationMeta);

            var creationMeta = new ReportCreationMetaData()
            {
                Name = reportRequest.Name,
                Description = reportRequest.Description,
                ProjectId = reportRequest.ProjectId,
                ReportTypeId = reportRequest.ReportTypeId,
                CurrentUser = user,
                JobId = job.Id,
                ModelId = reportRequest.ModelId
            };

            var report = await reportManager.Create(creationMeta);

            var reportDatasetCreationMeta = new ReportDatasetCreationMetaData()
            {
                ReportId = report.Id,
                ReportConfigurationId = report.ReportConfigurationId,
                DatasetConfig = reportRequest.DatasetConfig,
                CurrentUser = user
            };

            var _ = await reportDatasetManager.Create(reportDatasetCreationMeta);

            return Ok(JsonConvert.SerializeObject(report));
        }

        [HttpPost("update", Name = nameof(ReportController.UpdateReport))]
        public async Task<IActionResult> UpdateReport(UpdateReportRequest reportRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var updateMeta = new ReportUpdateMetaData()
            {
                Id = reportRequest.Id,
                Name = reportRequest.Name,
                Description = reportRequest.Description,
                ProjectId = reportRequest.ProjectId,
                ReportTypeId = reportRequest.ReportTypeId,
                CurrentUser = user,
                JobFreqTypeId = reportRequest.JobFreqTypeId,
                DatasetConfig = reportRequest.DatasetConfig,
                ModelId = reportRequest.ModelId
            };

            var report = await reportManager.Update(updateMeta);

            return Ok(JsonConvert.SerializeObject(report));
        }

        [HttpGet("{reportId}/getReport", Name = nameof(GetReport))]
        public async Task<IActionResult> GetReport([FromRoute] Guid reportId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new ReportKey()
            {
                Id = reportId,
                CurrentUser = user
            };

            try
            {
                var report = await reportManager.Get(key);

                return Ok(JsonConvert.SerializeObject(report));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving project. {ex}");
            }
        }

        [HttpGet("getAllReports", Name = nameof(ReportController.GetAllReports))]
        public async Task<IActionResult> GetAllReports()
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return this.BadRequest("User not found.");
                }

                var key = new ReportKey()
                {
                    CurrentUser = user
                };

                var projects = await reportManager.GetAll(key);

                return Ok(JsonConvert.SerializeObject(projects));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }

        // [HttpGet("getAllReportTypes", Name = nameof(ReportController.GetAllReportTypes))]
        // public async Task<IActionResult> GetAllReportTypes()
        // {
        //     try
        //     {
        //         var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //
        //         if (user == null)
        //         {
        //             return this.BadRequest("User not found.");
        //         }
        //
        //         var key = new ReportKey()
        //         {
        //             CurrentUser = user
        //         };
        //
        //         var reportTypes = await reportManager.GetAllReportTypes(key);
        //
        //         return Ok(JsonConvert.SerializeObject(reportTypes));
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log the exception or handle it appropriately
        //         return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
        //     }
        // }

        [HttpGet("{projectId}/getAllReportsForProject", Name = nameof(GetAllReportsForProject))]
        public async Task<IActionResult> GetAllReportsForProject([FromRoute] Guid projectId)
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return this.BadRequest("User not found.");
                }

                var key = new ReportKey()
                {
                    CurrentUser = user,
                    ProjectId = projectId
                };

                var projects = await reportManager.GetAllReportsFromProject(key); // was getAll

                return Ok(JsonConvert.SerializeObject(projects));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("{reportId}/softDeleteReport", Name = nameof(ReportController.SoftDeleteReport))]
        public async Task<IActionResult> SoftDeleteReport([FromRoute] Guid reportId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }
            
            var key = new ReportKey()
            {
                Id = reportId,
                CurrentUser = user
            };

            var success = await reportManager.SoftDelete(key);

            return Ok(success);
        }

        [HttpGet("{reportId}/getReportDomainData", Name = nameof(GetReportDomainData))]
        public async Task<IActionResult> GetReportDomainData([FromRoute] Guid reportId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new ReportKey()
            {
                Id = reportId,
                CurrentUser = user
            };

            try
            {
                var domainOptions = await reportManager.GetReportDomainOptions(key);

                return Ok(JsonConvert.SerializeObject(domainOptions));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving domain options. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("getDatasetData", Name = nameof(ReportController.GetDatasetData))]
        public async Task<IActionResult> GetDatasetData(
            [FromBody] DatasetDWHQueryingParameters datasetQueryingParameters)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new ReportKey()
            {
                Id = datasetQueryingParameters.ReportId,
                CurrentUser = user
            };

            var report = await reportManager.Get(key);

            if (report == null)
            {
                throw new Exception("Report not found.");
            }

            // // TODO enforce reportId and user checks
            // var datasetQueryingParameters = new DatasetDWHQueryingParameters()
            // {
            //     DatasetId = datasetId,
            //     PeriodStart = periodStart,
            //     PeriodEnd = periodEnd,
            //     Attributes = attributes,
            //     KPIs = kpis,
            //     RowLimit = rowLimit,
            //     ReportId = reportId
            // };

            var response = await datasetManager.GetData(datasetQueryingParameters, user);

            return Ok(response);
        }
        
    }
}

/*
 * TODO Add Configuration for dataset entities
 * TODO When using a specific dataset, we will need to modify the DWService to select the correct instance [x]
 * TODO run migrations
 */