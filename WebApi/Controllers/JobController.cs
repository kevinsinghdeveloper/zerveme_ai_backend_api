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
    using IJobResourceManager = zervemedata.Core.Contracts.Abstractions.IJobResourceManager<
        zervemedata.Data.DataModels.Data.JobInfo,
        zervemedata.Data.DataModels.Data.JobFreqTypeInfo,
        zervemedata.Data.Entities.Keys.JobKey,
        zervemedata.Data.DataModels.CreateMeta.JobCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.JobUpdateMetaData,
        zervemedata.Data.DataModels.UpdateMeta.JobScheduleUpdateMetaData
    >;

    [Authorize(Policy = RoleConstants.User)]
    [Route("api/Job")]
    [ApiController]
    public class JobController(IJobResourceManager jobManager) : ControllerBase
    {
        [HttpPost("create", Name = nameof(JobController.CreateJob))]
        public async Task<IActionResult> CreateJob(CreateJobRequest jobRequest)
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

            var creationMeta = new JobCreationMetaData()
            {
                JobFreqTypeId = jobRequest.JobFreqTypeId,
                CurrentUser = user
            };

            var job = await jobManager.Create(creationMeta);

            return Ok(JsonConvert.SerializeObject(job));
        }

        [HttpPost("update", Name = nameof(JobController.UpdateJob))]
        public async Task<IActionResult> UpdateJob(UpdateJobRequest jobRequest)
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

            var updateMeta = new JobUpdateMetaData()
            {
                Id = jobRequest.Id,
                JobFreqTypeId = jobRequest.JobFreqTypeId,
                CurrentUser = user
            };

            var job = await jobManager.Update(updateMeta);

            return Ok(JsonConvert.SerializeObject(job));
        }

        [HttpGet("{jobId}/getJob", Name = nameof(GetJob))]
        public async Task<IActionResult> GetJob([FromRoute] Guid jobId)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new JobKey()
            {
                Id = jobId,
                CurrentUser = user
            };

            try
            {
                var job = await jobManager.Get(key);

                return Ok(JsonConvert.SerializeObject(job));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving project. {ex}");
            }
        }

        [HttpGet("getAllJobs", Name = nameof(JobController.GetAllJobs))]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return this.BadRequest("User not found.");
                }

                var key = new JobKey()
                {
                    CurrentUser = user
                };

                var jobs = await jobManager.GetAll(key);

                return Ok(JsonConvert.SerializeObject(jobs));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }

        [HttpGet("getAllJobFreqTypes", Name = nameof(JobController.GetAllJobFreqTypes))]
        public async Task<IActionResult> GetAllJobFreqTypes()
        {
            try
            {
                var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (user == null)
                {
                    return this.BadRequest("User not found.");
                }

                var key = new JobKey()
                {
                    CurrentUser = user
                };

                var jobs = await jobManager.GetAllJobFreqTypes(key);

                return Ok(JsonConvert.SerializeObject(jobs));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving projects. {ex}");
            }
        }

        [HttpPost("startJob", Name = nameof(JobController.StartJob))]
        public async Task<IActionResult> StartJob(UpdateJobScheduleRequest jobScheduleRequest)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new JobKey()
            {
                Id = jobScheduleRequest.JobId,
                CurrentUser = user,
                StatusLog = jobScheduleRequest.StatusLog
            };

            try
            {
                var pass = await jobManager.StartJob(key);

                return Ok(pass);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while changing job status. {ex}");
            }
        }

        [HttpPost("completeJob", Name = nameof(JobController.CompleteJob))]
        public async Task<IActionResult> CompleteJob(UpdateJobScheduleRequest jobScheduleRequest)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new JobKey()
            {
                Id = jobScheduleRequest.JobId,
                CurrentUser = user,
                StatusLog = jobScheduleRequest.StatusLog
            };

            try
            {
                var pass = await jobManager.CompleteJob(key);

                return Ok(pass);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while changing job status. {ex}");
            }
        }

        [HttpPost("queueJob", Name = nameof(JobController.QueueJob))]
        public async Task<IActionResult> QueueJob(UpdateJobScheduleRequest jobScheduleRequest)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new JobKey()
            {
                Id = jobScheduleRequest.JobId,
                CurrentUser = user,
                StatusLog = jobScheduleRequest.StatusLog
            };

            try
            {
                var pass = await jobManager.QueueJob(key);

                return Ok(pass);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while changing job status. {ex}");
            }
        }

        [HttpPost("cancelJob", Name = nameof(JobController.CancelJob))]
        public async Task<IActionResult> CancelJob(UpdateJobScheduleRequest jobScheduleRequest)
        {
            var user = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (user == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new JobKey()
            {
                Id = jobScheduleRequest.JobId,
                CurrentUser = user,
                StatusLog = jobScheduleRequest.StatusLog
            };

            try
            {
                var pass = await jobManager.CancelJob(key);

                return Ok(pass);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while changing job status. {ex}");
            }
        }
    }
}

/*
 * TODO Add Configuration for dataset entities
 * TODO When using a specific dataset, we will need to modify the DWService to select the correct instance [x]
 * TODO run migrations
 */