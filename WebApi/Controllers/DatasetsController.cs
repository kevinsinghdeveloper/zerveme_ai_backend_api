using zervemedata.Data.Entities.Keys;

namespace zervemedata.api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data.DataModels.Requests;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Authorization;
    using zervemedata.Data.DataModels.Querying;
    using zervemedata.Data.Enumerations;
    
    using IDatasetResourceManager = zervemedata.Core.Contracts.Abstractions.IDatasetResourceManager<
        zervemedata.Data.DataModels.CreateMeta.DatasetCreationMetaData,
        zervemedata.Data.Entities.Keys.DatasetKey,
        zervemedata.Data.DataModels.CreateMeta.DatasetUpdateMetaData,
        zervemedata.Data.DataModels.Data.DatasetData, zervemedata.Data.DataModels.Querying.DatasetDWHQueryingParameters,
        zervemedata.Data.DataModels.Responses.DatasetQueriedData,
        zervemedata.Data.DataModels.Responses.DatasetTemplateQueryData>;

    [Route("api/datasets")]
    [ApiController]
    public class DatasetsController(IDatasetResourceManager datasetManager) : ControllerBase
    {
        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("create", Name = nameof(DatasetsController.CreateDataset))]
        public async Task<IActionResult> CreateDataset(CreateDatasetRequest datasetRequest)
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


            var datasetCreationMetaData = new DatasetCreationMetaData()
            {
                Name = datasetRequest.Name,
                VizResponseData = datasetRequest.VizResponseData,
                DomainData = datasetRequest.DomainData,
                DWHId = datasetRequest.DwhId,
                ReportId = datasetRequest.ReportId,
                CurrentUser = user
            };

            var dataset = await datasetManager.Create(datasetCreationMetaData);

            return Ok(JsonConvert.SerializeObject(dataset));
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("update", Name = nameof(DatasetsController.UpdateDataset))]
        public async Task<IActionResult> UpdateDataset(UpdateDatasetRequest datasetRequest,
            bool autoGenDateRefreshDate = false)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var datasetUpdateMetaData = new DatasetUpdateMetaData()
            {
                Id = datasetRequest.Id,
                Name = datasetRequest.Name,
                DomainData = datasetRequest.DomainData,
                DWHId = datasetRequest.DWHId,
                VizResponseData = datasetRequest.VizResponseData,
                // DatasetEtlConfigObject = datasetRequest.DatasetEtlConfiguration,
                DataRefreshedDate = autoGenDateRefreshDate ? DateTime.Now : datasetRequest.DataRefreshedDate,
                ReportId = datasetRequest.ReportId
            };

            var model = await datasetManager.Update(datasetUpdateMetaData);
            
            return Ok(model);
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpGet("getDatasetForEtl", Name = nameof(DatasetsController.GetDatasetForEtl))]
        public async Task<IActionResult> GetDatasetForEtl(Guid Id)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new DatasetKey()
            {
                Id = Id,
                CurrentUser = user
            };
            try
            {
                var dataset = await datasetManager.GetDatasetWithEtlConfig(key);

                return Ok(JsonConvert.SerializeObject(dataset));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving Dataset. {ex}");
            }
        }
        
        [Authorize(Policy = RoleConstants.User)]
        [HttpGet("get", Name = nameof(DatasetsController.GetDataset))]
        public async Task<IActionResult> GetDataset(Guid Id)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new DatasetKey()
            {
                Id = Id,
                CurrentUser = user
            };
            try
            {
                var dataset = await datasetManager.Get(key);

                return Ok(JsonConvert.SerializeObject(dataset));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving Dataset. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.User)]
        [HttpGet("getDatasetDomainOptions", Name = nameof(DatasetsController.GetDatasetDomainOptions))]
        public async Task<IActionResult> GetDatasetDomainOptions(Guid Id)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new DatasetKey()
            {
                Id = Id,
                CurrentUser = user
            };
            try
            {
                var dataset = await datasetManager.GetDomainOptions(key);

                return Ok(JsonConvert.SerializeObject(dataset));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving Dataset. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.User)]
        [HttpGet("getAllDatasetNames", Name = nameof(DatasetsController.GetAllDatasetNames))]
        public async Task<IActionResult> GetAllDatasetNames()
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new DatasetKey()
            {
                CurrentUser = user
            };
            try
            {
                var dataset = await datasetManager.GetAllDatasetNames(key);

                return Ok(JsonConvert.SerializeObject(dataset));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving Dataset. {ex}");
            }
        }


        [Authorize(Policy = RoleConstants.User)]
        [HttpGet("getall", Name = nameof(DatasetsController.GetAllDatasets))]
        public async Task<IActionResult> GetAllDatasets()
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new DatasetKey()
            {
                CurrentUser = user
            };
            try
            {
                var dataset = await datasetManager.GetAll(key);

                return Ok(JsonConvert.SerializeObject(dataset));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving Dataset. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("getdata", Name = nameof(DatasetsController.GetData))]
        public async Task<IActionResult> GetData([FromBody] DatasetDWHQueryingParameters datasetQueryingParameters)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var response = await datasetManager.GetData(datasetQueryingParameters, user);
            return Ok(response);
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("getvizdata", Name = nameof(DatasetsController.GetVizData))]
        public async Task<IActionResult> GetVizData([FromBody] DatasetDWHQueryingParameters datasetQueryingParameters)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var response = await datasetManager.GetVizData(datasetQueryingParameters, user);
            return Ok(response);
        }

        // TODO implement this method -> this will be called when we are focusing on a report
        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("getsampledata", Name = nameof(DatasetsController.GetSampleData))]
        public async Task<IActionResult> GetSampleData(Guid datasetId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var datasetQueryingParameters = new DatasetDWHQueryingParameters
            {
                DatasetId = datasetId,
                RowLimit = 100
            };

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