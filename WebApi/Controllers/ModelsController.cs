using zervemedata.Core.Contracts.Abstractions;
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
    using IModelsResourceManager = zervemedata.Core.Contracts.Abstractions.IModelsResourceManager<
        zervemedata.Data.DataModels.Data.ModelsInfo,
        zervemedata.Data.DataModels.Data.ModelTypeInfo,
        zervemedata.Data.Entities.Keys.ModelsKey,
        zervemedata.Data.DataModels.CreateMeta.ModelsCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ModelsUpdateMetaData>;

    [Authorize(Policy = RoleConstants.User)]
    [Route("api/models")]
    [ApiController]
    public class ModelsController(IModelsResourceManager modelsManager) : ControllerBase
    {
        [HttpPost("create", Name = nameof(ModelsController.CreateModel))]
        public async Task<IActionResult> CreateModel(CreateModelsRequest modelsRequest)
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

            var creationMeta = new ModelsCreationMetaData()
            {
                CurrentUser = user,
                Name = modelsRequest.Name,
                Description = modelsRequest.Description,
                ModelConfig = modelsRequest.ModelConfig,
                ModelTypeId = modelsRequest.ModelTypeId
            };

            var models = await modelsManager.Create(creationMeta);

            return Ok(JsonConvert.SerializeObject(models));
        }

        [HttpPost("update", Name = nameof(ModelsController.UpdateModel))]
        public async Task<IActionResult> UpdateModel(UpdateModelsRequest modelsRequest)
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

            var updateMeta = new ModelsUpdateMetaData()
            {
                CurrentUser = user,
                Id = modelsRequest.Id,
                Name = modelsRequest.Name,
                Description = modelsRequest.Description,
                ModelConfig = modelsRequest.ModelConfig,
                ModelTypeId = modelsRequest.ModelTypeId
            };

            var model = await modelsManager.Update(updateMeta);

            return Ok(JsonConvert.SerializeObject(model));
        }

        [HttpGet("{modelId}/getModel", Name = nameof(GetModel))]
        public async Task<IActionResult> GetModel([FromRoute] Guid modelId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new ModelsKey()
            {
                CurrentUser = user,
                Id = modelId
            };

            try
            {
                var model = await modelsManager.Get(key);

                return Ok(JsonConvert.SerializeObject(model));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving project. {ex}");
            }
        }

        [HttpGet("getAllModels", Name = nameof(ModelsController.GetAllModels))]
        public async Task<IActionResult> GetAllModels()
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return this.BadRequest("User not found.");
                }

                // TODO missing id of model
                var key = new ModelsKey()
                {
                    CurrentUser = user
                };

                var models = await modelsManager.GetAll(key);

                return Ok(JsonConvert.SerializeObject(models));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }

        [HttpGet("getAllModelTypes", Name = nameof(ModelsController.GetAllModelTypes))]
        public async Task<IActionResult> GetAllModelTypes()
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return this.BadRequest("User not found.");
                }

                // TODO missing id of model
                var key = new ModelsKey()
                {
                    CurrentUser = user
                };

                var models = await modelsManager.GetAllModelTypes(key);

                return Ok(JsonConvert.SerializeObject(models));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }
        
        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("{modelId}/softDeleteModel", Name = nameof(ModelsController.SoftDeleteModel))]
        public async Task<IActionResult> SoftDeleteModel([FromRoute] Guid modelId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new ModelsKey()
            {
                Id = modelId,
                CurrentUser = user
            };

            var success = await modelsManager.SoftDelete(key);

            return Ok(success);
        }
    }
}

/*
 * TODO Add Configuration for dataset entities
 * TODO When using a specific dataset, we will need to modify the DWService to select the correct instance [x]
 * TODO run migrations
 */