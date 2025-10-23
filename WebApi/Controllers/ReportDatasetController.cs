using zervemedata.Data.DataModels.UpdateMeta;
using zervemedata.Data.Entities.Keys;

namespace api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data.DataModels.Requests;
    using zervemedata.Data.Enumerations;
    using Microsoft.AspNetCore.Authorization;
    using IReportDatasetResourceManager = zervemedata.Core.Contracts.Abstractions.IReportDatasetResourceManager<
        zervemedata.Data.DataModels.Data.ReportDatasetInfo,
        ReportDatasetKey,
        zervemedata.Data.DataModels.CreateMeta.ReportDatasetCreationMetaData,
        ReportDatasetUpdateMetaData>;
    using IReportConfigurationResourceManager =
        zervemedata.Core.Contracts.Abstractions.IReportConfigurationResourceManager<
            zervemedata.Data.DataModels.Data.ReportConfigurationInfo,
            zervemedata.Data.DataModels.Data.ReportTypeInfo,
            zervemedata.Data.Entities.Keys.ReportConfigurationKey,
            zervemedata.Data.Entities.Keys.ReportKey,
            zervemedata.Data.DataModels.CreateMeta.ReportConfigurationCreationMetaData,
            zervemedata.Data.DataModels.CreateMeta.ReportTypeCreationMetaData,
            zervemedata.Data.DataModels.UpdateMeta.ReportConfigurationUpdateMetaData>;
    [Route("api/reportdataset")]
    [ApiController]
    public class ReportDatasetController(
        IReportDatasetResourceManager reportDatasetManager,
        IReportConfigurationResourceManager reportConfigManager)
        : ControllerBase
    {
        [Authorize(Policy = RoleConstants.SuperAdmin)]
        [HttpPost("createReportConfiguration", Name = nameof(ReportDatasetController.CreateReportConfiguration))]
        public async Task<IActionResult> CreateReportConfiguration(CreateReportConfigurationRequest reportConfigRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var reportConfigCreationMeta = new ReportConfigurationCreationMetaData()
            {
                ReportTypeId = reportConfigRequest.ReportTypeId,
                ReportConfig = reportConfigRequest.ReportConfig,
                CurrentUser = user
            };

            var reportConfig = await reportConfigManager.Create(reportConfigCreationMeta);

            return Ok(JsonConvert.SerializeObject(reportConfig));
        }

        [Authorize(Policy = RoleConstants.SuperAdmin)]
        [HttpPost("createReportType", Name = nameof(ReportDatasetController.CreateReportType))]
        public async Task<IActionResult> CreateReportType(CreateReportTypeRequest reportTypeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var reportTypeCreationMeta = new ReportTypeCreationMetaData()
            {
                Name = reportTypeRequest.Name,
                Description = reportTypeRequest.Description,
                VizTemplate = reportTypeRequest.VizTemplate,
                ReportConfig = reportTypeRequest.ReportConfig,
                CurrentUser = user
            };

            var reportConfig = await reportConfigManager.CreateReportType(reportTypeCreationMeta);

            return Ok(JsonConvert.SerializeObject(reportConfig));
        }
    
        [HttpPost("createReportDataset", Name = nameof(ReportDatasetController.CreateReportDataset))]
        public async Task<IActionResult> CreateReportDataset(CreateReportDatasetRequest reportDatasetRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var reportDatasetCreationMeta = new ReportDatasetCreationMetaData()
            {
                ReportConfigurationId = reportDatasetRequest.ReportConfigurationId,
                ReportId = reportDatasetRequest.ReportId, // TODO change to reportId
                DatasetConfig = reportDatasetRequest.DatasetConfig,
                CurrentUser = user
            };

            var reportDataset = await reportDatasetManager.Create(reportDatasetCreationMeta);

            return Ok(JsonConvert.SerializeObject(reportDataset));
        }

        [HttpPost("updateReportConfiguration", Name = nameof(ReportDatasetController.UpdateReportConfiguration))]
        public async Task<IActionResult> UpdateReportConfiguration(UpdateReportConfigurationRequest reportConfigRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var updateMeta = new ReportConfigurationUpdateMetaData()
            {
                Id = reportConfigRequest.Id,
                ReportTypeId = reportConfigRequest.ReportTypeId,
                ReportConfig = reportConfigRequest.ReportConfig,
                CurrentUser = user
            };

            var reportConfig = await reportConfigManager.Update(updateMeta);

            return Ok(JsonConvert.SerializeObject(reportConfig));
        }

        [HttpPost("updateReportDataset", Name = nameof(ReportDatasetController.UpdateReportDataset))]
        public async Task<IActionResult> UpdateReportDataset(UpdateReportDatasetRequest reportDatasetRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var updateMeta = new ReportDatasetUpdateMetaData()
            {
                Id = reportDatasetRequest.Id,
                ReportConfigurationId = reportDatasetRequest.ReportConfigurationId,
                ReportId = reportDatasetRequest.ReportId,
                DatasetConfig = reportDatasetRequest.DatasetConfig,
                CurrentUser = user
            };

            var report = await reportDatasetManager.Update(updateMeta);

            return Ok(JsonConvert.SerializeObject(report));
        }

        [HttpGet("{reportConfigurationId}/getReportConfiguration", Name = nameof(GetReportConfiguration))]
        public async Task<IActionResult> GetReportConfiguration([FromRoute] Guid reportConfigurationId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var key = new ReportConfigurationKey()
            {
                Id = reportConfigurationId,
                CurrentUser = user
            };

            try
            {
                var reportConfig = await reportConfigManager.Get(key);

                return Ok(JsonConvert.SerializeObject(reportConfig));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving project. {ex}");
            }
        }

        [HttpGet("{reportDatasetId}/getReportDataset", Name = nameof(GetReportDataset))]
        public async Task<IActionResult> GetReportDataset([FromRoute] Guid reportDatasetId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var key = new ReportDatasetKey()
            {
                Id = reportDatasetId,
                CurrentUser = user
            };

            try
            {
                var reportDataset = await reportDatasetManager.Get(key);

                return Ok(JsonConvert.SerializeObject(reportDataset));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving project. {ex}");
            }
        }

        [HttpGet("getAllReportConfigurations", Name = nameof(ReportDatasetController.GetAllReportConfigurations))]
        public async Task<IActionResult> GetAllReportConfigurations()
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var key = new ReportConfigurationKey()
                {
                    CurrentUser = user
                };

                var reportConfigs = await reportConfigManager.GetAll(key);

                return Ok(JsonConvert.SerializeObject(reportConfigs));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }

        [HttpGet("getAllReportDatasets", Name = nameof(ReportDatasetController.GetAllReportDatasets))]
        public async Task<IActionResult> GetAllReportDatasets()
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var key = new ReportDatasetKey()
                {
                    CurrentUser = user
                };

                var reportDatasets = await reportDatasetManager.GetAll(key);

                return Ok(JsonConvert.SerializeObject(reportDatasets));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("{reportConfigurationId}/softDeleteReportConfiguration",
            Name = nameof(ReportDatasetController.SoftDeleteReportConfiguration))]
        public async Task<IActionResult> SoftDeleteReportConfiguration([FromRoute] Guid reportConfigurationId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var key = new ReportConfigurationKey()
            {
                Id = reportConfigurationId,
                CurrentUser = user
            };

            var success = await reportConfigManager.SoftDelete(key);

            return Ok(success);
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("{reportDatasetId}/softDeleteReportDataset",
            Name = nameof(ReportDatasetController.SoftDeleteReportDataset))]
        public async Task<IActionResult> SoftDeleteReportDataset([FromRoute] Guid reportDatasetId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var key = new ReportDatasetKey()
            {
                Id = reportDatasetId,
                CurrentUser = user
            };

            var success = await reportDatasetManager.SoftDelete(key);

            return Ok(success);
            throw new NotImplementedException("Soft delete not implemented yet.");
        }

        [HttpGet("getAllReportTypes", Name = nameof(ReportDatasetController.GetAllReportTypes))]
        public async Task<IActionResult> GetAllReportTypes()
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

                var reportTypes = await reportConfigManager.GetAllReportTypes(key);

                return Ok(JsonConvert.SerializeObject(reportTypes));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }
    }
}

// Example of report config input
// {
//      "reportTypeId": "458e4673-53c9-4120-b7b3-08dd78887700",
//      "reportConfig": {
//        "fields": [
//          {
//            "fieldName": "Tags",
//            "possibleOptions": [
//              "Technology", "Finance", "Clothing", "Retail"
//            ]
//          },
//    {
//            "fieldName": "Target Location",
//            "possibleOptions": [
//            ]
//          },
//    {
//            "fieldName": "Your Company Name",
//            "possibleOptions": [
//            ]
//          },
//    {
//            "fieldName": "Top Competitors",
//            "possibleOptions": [
//            ],
//    "FieldType": 1
//          }
//    
//        ]
//      }
//    }
