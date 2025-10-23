using zervemedata.Core.Contracts.Abstractions;
using zervemedata.Data.Entities.Keys;

namespace zervemedata.api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data.DataModels.Requests;
    using zervemedata.Data.Enumerations;
    using Microsoft.AspNetCore.Authorization;
    using IOrganizationResourceManager = zervemedata.Core.Contracts.Abstractions.IOrganizationResourceManager<
        zervemedata.Data.DataModels.Data.OrganizationsInfo,
        zervemedata.Data.DataModels.CreateMeta.OrganizationCreationMetaData,
        zervemedata.Data.Entities.Keys.OrganizationKey,
        zervemedata.Data.DataModels.UpdateMeta.OrganizationUpdateMetaData>;

    [Route("api/organization")]
    [ApiController]
    public class OrganizationController(IOrganizationResourceManager orgManager) : ControllerBase
    {
        [Authorize(Policy = RoleConstants.SuperAdmin)]
        [HttpPost("create", Name = nameof(OrganizationController.CreateOrganization))]
        public async Task<IActionResult> CreateOrganization(CreateOrganizationRequest organizationRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var creationMeta = new OrganizationCreationMetaData()
            {
                Name = organizationRequest.Name,
                Address1 = organizationRequest.Address1,
                Address2 = organizationRequest.Address2,
                Address3 = organizationRequest.Address3,
                City = organizationRequest.City,
                State = organizationRequest.State,
                Country = organizationRequest.Country,
                ZipCode = organizationRequest.ZipCode,
                PrimaryPhone = organizationRequest.PrimaryPhone,
                SecondaryPhone = organizationRequest.SecondaryPhone,
                NotificationEmail = organizationRequest.NotificationEmail,
                CurrentUser = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };

            var org = await orgManager.Create(creationMeta);

            return Ok(JsonConvert.SerializeObject(org));
            ;
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpGet("{organizationId}/getOrganization", Name = nameof(GetOrganization))]
        public async Task<IActionResult> GetOrganization([FromRoute] Guid organizationId)
        {
            var key = new OrganizationKey()
            {
                Id = organizationId
            };

            try
            {
                var org = await orgManager.Get(key);

                return Ok(JsonConvert.SerializeObject(org));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving organization. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.SuperAdmin)]
        [HttpGet("getall", Name = nameof(OrganizationController.GetAllOrganizations))]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var models = await orgManager.GetAll();

            return Ok(JsonConvert.SerializeObject(models));
        }

        // [HttpPost("update", Name = nameof(DWHController.UpdateDWH))]
        // public async Task<IActionResult> UpdateOrganization(UpdateDWHRequest dwhRequest)
        // {
        //     if (!this.ModelState.IsValid)
        //     {
        //         return this.BadRequest(this.ModelState);
        //     }
        //
        //     var updateMeta = new DWHUpdateMetaData()
        //     {
        //         Id = dwhRequest.Id,
        //         Name = dwhRequest.Name,
        //         Schema = dwhRequest.Schema,
        //         ConnectionString = dwhRequest.ConnectionString,
        //         DWH = dwhRequest.DWH
        //     };
        //
        //     var model = await this.orgManager.Update(updateMeta);
        //
        //     return Ok(model);
        // }
        //
    }
}

/*
 * TODO Add Configuration for dataset entities
 * TODO When using a specific dataset, we will need to modify the DWService to select the correct instance [x]
 * TODO run migrations
 */