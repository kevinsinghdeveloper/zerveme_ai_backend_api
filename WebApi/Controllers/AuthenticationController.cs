namespace zervemedata.api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using zervemedata.Data.DataModels.CreateMeta;
    using zervemedata.Data.DataModels.Requests;
    using zervemedata.Core.Services;
    
    using IAuthenticationResourceManager = zervemedata.Core.Contracts.Abstractions.IAuthenticationResourceManager<
        zervemedata.Data.DataModels.CreateMeta.UserResponseMetaData, zervemedata.Data.DataModels.Responses.AuthUserResponse >;

    
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController(IAuthenticationResourceManager authenticationManager) : ControllerBase
    {
        [HttpPost("authorizeUser", Name = nameof(AuthenticationController.AuthorizeUser))]
        public async Task<IActionResult> AuthorizeUser(AuthUserRequest userRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var authUserMeta = new UserResponseMetaData()
            {
                userNameOrEmail = userRequest.UserNameOrEmail,
                password = userRequest.Password
            };

            var data = await authenticationManager.Authenticate(authUserMeta);

            return Ok(data);
        }
        // DO NOT IMPLEMENT LOG OUT... HANDLED ON CLIENT
    }
}