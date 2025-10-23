namespace zervemedata.api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data.DataModels.Requests;
    using zervemedata.Data.Enumerations;
    using Microsoft.AspNetCore.Authorization;
    
    using IDwResourceManager = zervemedata.Core.Contracts.Abstractions.IDwResourceManager<
        zervemedata.Data.DataModels.CreateMeta.DWHCreationMetaData, zervemedata.Data.DataModels.CreateMeta.DWHUpdateMetaData,
        zervemedata.Data.DataModels.Data.DWHData>;
    
    [Route("api/dwh")]
    [ApiController]
    [Authorize(Policy = RoleConstants.Admin)]
    public class DWHController(IDwResourceManager dwhManager) : ControllerBase
    {
        [HttpPost("create", Name = nameof(DWHController.CreateDWH))]
        public async Task<IActionResult> CreateDWH(CreateDWHRequest dwhRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var creationMeta = new DWHCreationMetaData()
            {
                Name = dwhRequest.Name,
                Schema = dwhRequest.Schema,
                ConnectionString = dwhRequest.ConnectionString,
                DWH = dwhRequest.DWH
            };

            var ok = await dwhManager.Create(creationMeta);
            
            return Ok(ok);
        }
        
        [HttpPost("update", Name = nameof(DWHController.UpdateDWH))]
        public async Task<IActionResult> UpdateDWH(UpdateDWHRequest dwhRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var updateMeta = new DWHUpdateMetaData()
            {
                Id = dwhRequest.Id,
                Name = dwhRequest.Name,
                Schema = dwhRequest.Schema,
                ConnectionString = dwhRequest.ConnectionString,
                DWH = dwhRequest.DWH
            };

            var model = await dwhManager.Update(updateMeta);
            
            return Ok(model);
        }
        
        [HttpGet("get", Name = nameof(DWHController.GetDWH))]
        public async Task<IActionResult> GetDWH(Guid Id)
        {
            var model = await dwhManager.Get(Id);
            
            return Ok(model);
        }
        
        [HttpGet("getall", Name = nameof(DWHController.GetAllDWH))]
        public async Task<IActionResult> GetAllDWH()
        {
            var models = await dwhManager.GetAll();
            
            return Ok(JsonConvert.SerializeObject(models));
        }
        
        [HttpPost("swapschemas", Name = nameof(DWHController.SwapSchemas))]
        public async Task<IActionResult> SwapSchemas(Guid Id)
        {
            var model = await dwhManager.SwapSchemas(Id);
            
            return Ok(model);
        }
    }
}

/*
 * TODO Add Configuration for dataset entities
 * TODO When using a specific dataset, we will need to modify the DWService to select the correct instance [x]
 * TODO run migrations
*/