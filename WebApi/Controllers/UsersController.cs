using Google.Apis.Util;
using zerveme.Utilities;
using zervemedata.Data.DataModels.UpdateMeta;

namespace zervemedata.api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using zervemedata.Core.Services;
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data.DataModels.Requests;
    using zervemedata.Data.Enumerations;
    using zervemedata.Data.Entities.Keys;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Authorization;
    
    using IUserResourceManager = zervemedata.Core.Contracts.Abstractions.IUserResourceManager<
        zervemedata.Data.DataModels.Data.UserInfo,
        zervemedata.Data.Entities.Keys.UserKey,
        zervemedata.Data.DataModels.CreateMeta.UserCreationMetaData,
        zervemedata.Data.DataModels.UpdateMeta.UserUpdateMetaData>;
    
    using IUserRolesResourceManager = zervemedata.Core.Contracts.Abstractions.IUserRolesResourceManager<
        zervemedata.Data.DataModels.Data.RoleInfo>;

    [Route("api/users")]
    [ApiController]
    public class UsersController(IUserResourceManager userManager, IUserRolesResourceManager roleManager)
        : ControllerBase
    {
        private readonly UsersResourceManager userManager = (UsersResourceManager)userManager;
        private readonly UsersRolesResourceManager roleManager = (UsersRolesResourceManager)roleManager;

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("create", Name = nameof(UsersController.CreateUser))]
        public async Task<IActionResult> CreateUser(CreateUserRequest userRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (userRequest.Password != userRequest.VerifyPassword)
            {
                return this.BadRequest("Password mismatch!");
            }

            var creatorUser = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (creatorUser == null)
            {
                return this.BadRequest("User not found.");
            }
            
            var userCreationMeta = new UserCreationMetaData()
            {
                UserName = userRequest.UserName,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                EmailAddress = userRequest.EmailAddress,
                Password = userRequest.Password,
                Roles = userRequest.Roles ??
                [
                    Role.User
                ], //Enum.TryParse(userRequest.RoleName, out Role parsedRole) ? parsedRole : Role.User, // multiple roles
                Address1 = userRequest.Address1,
                Address2 = userRequest.Address2,
                Address3 = userRequest.Address3,
                City = userRequest.City,
                State = userRequest.State,
                Country = userRequest.Country,
                ZipCode = userRequest.ZipCode,
                PrimaryPhone = userRequest.PrimaryPhone,
                SecondaryPhone = userRequest.SecondaryPhone,
                NotificationEmail = userRequest.NotificationEmail,
                CurrentUser = creatorUser
            };

            var user = await this.userManager.Create(userCreationMeta);

            return Ok(JsonConvert.SerializeObject(user));
        }
        
        [Authorize(Policy = RoleConstants.Admin)]
        [HttpGet("getAllUsers", Name = nameof(UsersController.GetAll))]
        public async Task<IActionResult> GetAll()
        {
            var cUser = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (cUser == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new UserKey
            {
                CurrentUser = cUser
            };
            
            try
            {
                var users = await userManager.GetAll(key);

                return Ok(JsonConvert.SerializeObject(users));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving users. {ex}");
            }
        }
        
        [Authorize(Policy = RoleConstants.Admin)]
        [HttpGet("getAllRoles", Name = nameof(UsersController.GetAllRoles))]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await roleManager.GetAll();

                return Ok(JsonConvert.SerializeObject(roles));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving roles. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("{userId}/softDeleteUser", Name = nameof(UsersController.SoftDeleteUser))]
        public async Task<IActionResult> SoftDeleteUser([FromRoute] Guid userId)
        {
            var key = new UserKey()
            {
                Id = userId.ToString()
            };

            var success = await userManager.SoftDeleteUser(key);
            
            return Ok(success);
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpPost("update", Name = nameof(UpdateUser))]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest userUpdateRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var creatorUser = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (creatorUser == null)
            {
                return this.BadRequest("User not found.");
            }

            var userUpdateMetaData = new UserUpdateMetaData()
            {
                Id = userUpdateRequest.Id,
                firstName = userUpdateRequest.firstName,
                lastName = userUpdateRequest.lastName,
                emailAddress = userUpdateRequest.emailAddress,
                userName = userUpdateRequest.userName,
                Address1 = userUpdateRequest.Address1,
                Address2 = userUpdateRequest.Address2,
                Address3 = userUpdateRequest.Address3,
                City = userUpdateRequest.City,
                State = userUpdateRequest.State,
                Country = userUpdateRequest.Country,
                ZipCode = userUpdateRequest.Address2,
                PrimaryPhone = userUpdateRequest.PrimaryPhone,
                SecondaryPhone = userUpdateRequest.SecondaryPhone,
                NotificationEmail = userUpdateRequest.NotificationEmail,
                CurrentUser = creatorUser
            };

            var user = await userManager.Update(userUpdateMetaData);

            return Ok(JsonConvert.SerializeObject(user));
        }

        [Authorize(Policy = RoleConstants.Admin)]
        [HttpGet("getUser", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser([FromQuery] Guid userId)
        {
            var cUser = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (cUser == null)
            {
                return this.BadRequest("User not found.");
            }
            
            var key = new UserKey
            {
                Id = userId == Guid.Empty ? null : userId.ToString(),
                CurrentUser = cUser
            };

            try
            {
                var user = await userManager.Get(key);

                return Ok(JsonConvert.SerializeObject(user));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving users. {ex}");
            }
        }

        [Authorize(Policy = RoleConstants.User)]
        [HttpGet("getCurrentUser", Name = nameof(GetCurrentUser))]
        public async Task<IActionResult> GetCurrentUser()
        {
            var cUser = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (cUser == null)
            {
                return this.BadRequest("User not found.");
            }

            var key = new UserKey
            {
                CurrentUser = cUser
            };

            try
            {
                var user = await userManager.Get(key);

                return Ok(JsonConvert.SerializeObject(user));
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, $"An error occurred while retrieving users. {ex}");
            }
        }
        // GET ORGANIZATIONS for user
        
        
    }
}