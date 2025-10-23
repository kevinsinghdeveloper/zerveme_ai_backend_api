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
    using IProjectResourceManager = zervemedata.Core.Contracts.Abstractions.IProjectResourceManager<
        zervemedata.Data.DataModels.Data.ProjectInfo,
        zervemedata.Data.Entities.Keys.ProjectKey,
        zervemedata.Data.DataModels.CreateMeta.ProjectCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.ProjectUpdateMetaData>;

    [Authorize(Policy = RoleConstants.User)]
    [Route("api/project")]
    [ApiController]
    public class ProjectController(IProjectResourceManager projectManager) : ControllerBase
    {
        [HttpPost("create", Name = nameof(ProjectController.CreateProject))]
        public async Task<IActionResult> CreateProject(CreateProjectRequest projectRequest)
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

            var creationMeta = new ProjectCreationMetaData()
            {
                Name = projectRequest.Name,
                Description = projectRequest.Description,
                OrganizationId = projectRequest.OrganizationId,
                CurrentUser = user
            };

            var project = await projectManager.Create(creationMeta);

            return Ok(JsonConvert.SerializeObject(project));
        }

        [HttpPost("update", Name = nameof(ProjectController.UpdateProject))]
        public async Task<IActionResult> UpdateProject(UpdateProjectRequest projectRequest)
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

            var updateMeta = new ProjectUpdateMetaData()
            {
                Id = projectRequest.Id,
                Name = projectRequest.Name,
                Description = projectRequest.Description,
                CurrentUser = user
            };

            var project = await projectManager.Update(updateMeta);

            return Ok(JsonConvert.SerializeObject(project));
        }

        [HttpGet("{projectId}/getProject", Name = nameof(GetProject))]
        public async Task<IActionResult> GetProject([FromRoute] Guid projectId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }
            
            var key = new ProjectKey()
            {
                Id = projectId,
                CurrentUser = user
            };

            try
            {
                var project = await projectManager.Get(key);

                return Ok(JsonConvert.SerializeObject(project));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving project. {ex}");
            }
        }

        [HttpGet("getAllOrgProjects", Name = nameof(ProjectController.GetAllOrgProjects))]
        public async Task<IActionResult> GetAllOrgProjects()
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return this.BadRequest("User not found.");
                }

                var key = new ProjectKey()
                {
                    CurrentUser = user
                };

                var projects = await projectManager.GetAllOrgProjects(key);

                return Ok(JsonConvert.SerializeObject(projects));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("{projectId}/softDeleteProject", Name = nameof(ProjectController.SoftDeleteProject))]
        public async Task<IActionResult> SoftDeleteProject([FromRoute] Guid projectId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }
            
            var key = new ProjectKey()
            {
                Id = projectId,
                CurrentUser = user
            };

            var success = await projectManager.SoftDelete(key);

            return Ok(success);
        }

    }
}

/*
 * TODO Add Configuration for dataset entities
 * TODO When using a specific dataset, we will need to modify the DWService to select the correct instance [x]
 * TODO run migrations
 */